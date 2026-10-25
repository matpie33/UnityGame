bl_info = {
    "name": "Specials menu",
    "author": "Mateusz",
    "version": (1, 2),
    "blender": (5, 0, 0),
    "location": "View3D > Tool & UV Editor > Tool",
    "category": "Add Mesh"
}

import bpy
import bmesh

# -------------------------------------------------------
# Panels
# -------------------------------------------------------

class SpecialsPanel(bpy.types.Panel):
    bl_label = "Specials panel"
    bl_idname = "VIEW3D_PT_SpecialsPanel"
    bl_space_type = "VIEW_3D"
    bl_region_type = "UI"
    bl_category = "Specials"
    
    def draw(self, context):
        layout = self.layout

        layout.operator("mesh.primitive_cube_add", icon='META_CUBE')
        layout.operator("object.move_to_view_center")

        layout.separator()

        box = layout.box()
        box.label(text="Flatten edge loops separately")

        row = box.row(align=True)
        split = row.split(factor=0.5)  

        sub = split.row(align=True)
        sub.prop(context.scene, "scale_zero_x", toggle=True, text="X")
        sub.prop(context.scene, "scale_zero_y", toggle=True, text="Y")
        sub.prop(context.scene, "scale_zero_z", toggle=True, text="Z")
        box.operator("mesh.scale_edge_loops_zero", icon='FULLSCREEN_EXIT')


class UvSpecialsPanel(bpy.types.Panel):
    bl_label = "Specials panel"
    bl_idname = "IMAGE_PT_UVSpecialsPanel"
    bl_space_type = "IMAGE_EDITOR"
    bl_region_type = "UI"
    bl_category = "Specials"
    
    def draw(self, context):
        layout = self.layout
        layout.operator("uv.make_rectangle", icon='MESH_GRID')


# -------------------------------------------------------
# Operators (3D Viewport)
# -------------------------------------------------------

class MoveToViewCenter(bpy.types.Operator):
    bl_idname = "object.move_to_view_center"
    bl_label = "Move to View Center"
    bl_options = {'REGISTER', 'UNDO'}

    target_position: bpy.props.FloatVectorProperty(size=3)

    def invoke(self, context, event):
        region_data = context.region_data
        if region_data is None:
            self.report({'WARNING'}, 'No view available')
            return {'CANCELLED'}

        self.target_position = region_data.view_location.copy()
        return self.execute(context)

    def execute(self, context):
        for obj in context.selected_objects:
            obj.location = self.target_position
        return {'FINISHED'}


class ScaleEdgeLoopsZero(bpy.types.Operator):
    bl_idname = "mesh.scale_edge_loops_zero"
    bl_label = "Flatten Edge Loops (Separate)"
    bl_description = "Scale each selected edge loop to 0 on chosen axis independently"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if context.mode != 'EDIT_MESH':
            self.report({'WARNING'}, "Edit Mode only")
            return {'CANCELLED'}

        obj = context.edit_object
        bm = bmesh.from_edit_mesh(obj.data)

        axes = []
        if context.scene.scale_zero_x:
            axes.append(0)
        if context.scene.scale_zero_y:
            axes.append(1)
        if context.scene.scale_zero_z:
            axes.append(2)

        if not axes:
            self.report({'WARNING'}, "No axis selected")
            return {'CANCELLED'}

        selected_edges = [e for e in bm.edges if e.select]
        visited = set()

        for edge in selected_edges:
            if edge in visited:
                continue

            stack = [edge]
            loop_edges = set()

            while stack:
                e = stack.pop()
                if e in loop_edges:
                    continue

                loop_edges.add(e)
                visited.add(e)

                for v in e.verts:
                    for linked in v.link_edges:
                        if linked.select and linked not in loop_edges:
                            stack.append(linked)

            verts = {v for e in loop_edges for v in e.verts}

            for axis in axes:
                median = sum(v.co[axis] for v in verts) / len(verts)
                for v in verts:
                    v.co[axis] = median

        bmesh.update_edit_mesh(obj.data)
        return {'FINISHED'}


# -------------------------------------------------------
# Operators (UV Editor)
# -------------------------------------------------------

class UvMakeRectangle(bpy.types.Operator):
    bl_idname = "uv.make_rectangle"
    bl_label = "Make Rectangle"
    bl_description = "Turn selected UV faces into an axis-aligned rectangle"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        if context.mode != 'EDIT_MESH':
            self.report({'WARNING'}, "Edit Mode only")
            return {'CANCELLED'}

        obj = context.edit_object
        bm = bmesh.from_edit_mesh(obj.data)
        
        uv_layer = bm.loops.layers.uv.verify()
        
        faces_modified = False
        for face in bm.faces:
            if face.select:
                uvs = [loop[uv_layer].uv for loop in face.loops]
                if not uvs:
                    continue
                
                min_x = min(uv.x for uv in uvs)
                max_x = max(uv.x for uv in uvs)
                min_y = min(uv.y for uv in uvs)
                max_y = max(uv.y for uv in uvs)
                
                if min_x == max_x or min_y == max_y:
                    continue
                
                cx = sum(uv.x for uv in uvs) / len(uvs)
                cy = sum(uv.y for uv in uvs) / len(uvs)
                
                for loop in face.loops:
                    uv = loop[uv_layer].uv
                    dx = 1 if uv.x >= cx else -1
                    dy = 1 if uv.y >= cy else -1
                    
                    uv.x = max_x if dx > 0 else min_x
                    uv.y = max_y if dy > 0 else min_y
                
                faces_modified = True

        if faces_modified:
            bmesh.update_edit_mesh(obj.data)
            self.report({'INFO'}, "UV faces shaped into rectangle")
            return {'FINISHED'}
        else:
            self.report({'WARNING'}, "No valid faces selected")
            return {'CANCELLED'}


# -------------------------------------------------------
# Register / Unregister
# -------------------------------------------------------

classes = (
    SpecialsPanel,
    UvSpecialsPanel,
    MoveToViewCenter,
    ScaleEdgeLoopsZero,
    UvMakeRectangle,
)

def register():
    for cls in classes:
        bpy.utils.register_class(cls)

    bpy.types.Scene.scale_zero_x = bpy.props.BoolProperty(name="X", default=False)
    bpy.types.Scene.scale_zero_y = bpy.props.BoolProperty(name="Y", default=False)
    bpy.types.Scene.scale_zero_z = bpy.props.BoolProperty(name="Z", default=False)


def unregister():
    for cls in reversed(classes):
        bpy.utils.unregister_class(cls)

    del bpy.types.Scene.scale_zero_x
    del bpy.types.Scene.scale_zero_y
    del bpy.types.Scene.scale_zero_z
    
if __name__ == "__main__":
    register()