using Gist2.Extensions.ScreenExt;
using LLGraphicsUnity.Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLGraphicsUnity {

	[ExecuteAlways]
	public class ThickLine : MonoBehaviour {

		public Presets presets = new();

		protected GLMaterial mat;

		#region unity
		protected void OnEnable() {
			mat = new GLMaterial();
		}
		protected void OnDisable() {
			mat.Dispose();
		}
		protected void OnRenderObject() {
			var c = Camera.current;
			if (mat == null || !isActiveAndEnabled) return;

            var data_always = new GLProperty() {
                Color = Color.white,
                MainTex = null,
                ZWriteMode = false,
                ZTestMode = GLProperty.ZTestEnum.ALWAYS,
            }; 
			var data_leq = new GLProperty() {
                Color = Color.white,
                MainTex = null,
                ZWriteMode = false,
                ZTestMode = GLProperty.ZTestEnum.LESSEQUAL,
            };
            var aspect = c.aspect;
			var size = 0.5f;

			if (c == Camera.main) {

				using (mat.GetScope(data_always))
				using (new GLMatrixScope()) {
					var rot = Quaternion.identity;
					var scale = new Vector3(size / aspect, size, 1f);

					GL.LoadIdentity();
					GL.LoadOrtho();

					using (mat.GetScope(new GLProperty(data_always) {
						Color = Color.magenta,
						LineThickness = 5f
					}, ShaderPass.Line))
					using (new GLModelViewScope(Matrix4x4.TRS(new Vector3(scale.x * 0.5f, scale.y * .5f, -1), rot, .8f * scale))) {
						Quad.LineStrip();
					}
					using (mat.GetScope(new GLProperty(data_always) {
						Color = Color.cyan,
						LineThickness = 10f
					}, ShaderPass.Line))
					using (new GLModelViewScope(Matrix4x4.TRS(new Vector3(scale.x * 0.5f, scale.y * 1.5f, -1), rot, .8f * scale))) {
						GL.Begin(GL.LINES);
						GL.Vertex3(-0.5f, -0.5f, 0f);
						GL.Vertex3(0.5f, 0.5f, 0f);
						GL.End();
					}
					using (mat.GetScope(new GLProperty(data_always) { Color = Color.red }))
					using (new GLModelViewScope(Matrix4x4.TRS(new Vector3(scale.x * 1.5f, scale.y * .5f, -1), rot, scale))) {
						Quad.TriangleStrip();
					}
				}
			}

			using (new GLMatrixScope()) {
				GL.LoadIdentity();
				GL.LoadProjectionMatrix(c.projectionMatrix);
				GL.modelview = c.worldToCameraMatrix;

				var cmain = Camera.main;
				var t = Time.time;
				var scale = cmain.orthographicSize * Vector3.one;

				var pos = cmain.IsometricUVToWorld(1.5f, 0.5f);
				var rot = Quaternion.Euler(new Vector3(-5f, 45 * t, -30f));
				var vm = Matrix4x4.TRS(pos, rot, 0.6f * scale);
				using (mat.GetScope(new GLProperty(data_always) {
					Color = Color.yellow,
					LineThickness = 5f
				}, ShaderPass.Line))
				using (new GLModelViewScope(vm)) {
					Box.Lines();
				}

				pos = cmain.IsometricUVToWorld(1.5f, 1.5f);
				rot = Quaternion.Euler(new Vector3(-5f, 45f * t + 15f, 0));
				vm = Matrix4x4.TRS(pos, rot, 0.8f * scale);
				using (mat.GetScope(new GLProperty(data_always) {
					Color = Color.grey,
					LineThickness = 5f
				}, ShaderPass.Line))
				using (new GLModelViewScope(vm)) {
					Circle.LineStrip(0.5f, 50);
				}

				pos = cmain.IsometricUVToWorld(0.5f, 0.5f);
				rot = Quaternion.Euler(new Vector3(-15f, 45f * t + 30f, 30f));
				vm = Matrix4x4.TRS(pos, rot, 0.4f * scale);
				using (mat.GetScope(new GLProperty(data_always) { Color = Color.green }))
				using (new GLModelViewScope(vm)) {
					Box.Triangles();
				}

                using (mat.GetScope(new GLProperty(data_leq) {
                    Color = Color.magenta,
                    LineThickness = 5f
                }, ShaderPass.Line)) {
					foreach (var tm in presets.shapes) {
						if (tm.transform == null) continue;

						using (new GLModelViewScope(tm.transform.localToWorldMatrix)) {
							Box.Lines();
						}
					}
				}
			}
		}
		#endregion

		#region declarations
		[System.Serializable]
		public class Presets {
            public Texture mainTex;
            public List<TransformShape> shapes = new();

			[System.Serializable]
			public class TransformShape {
				public Transform transform;
			}
		}
        #endregion
    }
}
