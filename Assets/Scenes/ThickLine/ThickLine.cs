using LLGraphicsUnity.Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LLGraphicsUnity {

	[ExecuteAlways]
	public class ThickLine : MonoBehaviour {

		public Texture mainTex;

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

			var data = new GLProperty() {
				Color = Color.white,
				MainTex = null,
				ZWriteMode = false,
				ZTestMode = GLProperty.ZTestEnum.ALWAYS,
			};
			var aspect = c.aspect;
			var size = 0.5f;

			if (c == Camera.main) {

				using (mat.GetScope(data))
				using (new GLMatrixScope()) {
					var rot = Quaternion.identity;
					var scale = new Vector3(size / aspect, size, 1f);

					GL.LoadIdentity();
					GL.LoadOrtho();

					using (mat.GetScope(new GLProperty(data) {
						Color = Color.magenta,
						LineThickness = 5f
					}, ShaderPass.Line))
					using (new GLModelViewScope(Matrix4x4.TRS(new Vector3(scale.x * 0.5f, scale.y * .5f, -1), rot, .8f * scale))) {
						Quad.LineStrip();
					}
					using (mat.GetScope(new GLProperty(data) {
						Color = Color.cyan,
						LineThickness = 10f
					}, ShaderPass.Line))
					using (new GLModelViewScope(Matrix4x4.TRS(new Vector3(scale.x * 0.5f, scale.y * 1.5f, -1), rot, .8f * scale))) {
						GL.Begin(GL.LINES);
						GL.Vertex3(-0.5f, -0.5f, 0f);
						GL.Vertex3(0.5f, 0.5f, 0f);
						GL.End();
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

				var pos = cmain.ViewportToWorldPoint(0.5f * new Vector3(1.5f / aspect, 0.5f));
				pos.z = 0f;
				var rot = Quaternion.Euler(new Vector3(-5f, 45 * t, 0f));
				var vm = Matrix4x4.TRS(pos, rot, 0.6f * scale);
				using (mat.GetScope(new GLProperty(data) {
					Color = Color.yellow,
					LineThickness = 5f
				}, ShaderPass.Line))
				using (new GLModelViewScope(vm)) {
					Box.Lines();
				}

				pos = cmain.ViewportToWorldPoint(0.5f * new Vector3(1.5f / aspect, 1.5f));
				pos.z = 0f;
				rot = Quaternion.Euler(new Vector3(-5f, 45f * t + 15f, 0f));
				vm = Matrix4x4.TRS(pos, rot, 0.8f * scale);
				using (mat.GetScope(new GLProperty(data) {
					Color = Color.grey,
					LineThickness = 5f
				}, ShaderPass.Line))
				using (new GLModelViewScope(vm)) {
					Circle.LineStrip(0.5f, 50);
				}
			}
		}
		#endregion
	}
}
