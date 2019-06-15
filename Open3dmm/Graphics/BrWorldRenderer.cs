//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Open3dmm.BRender;
//using Open3dmm.Classes;
//using System;
//using System.Collections.Generic;
//using System.Runtime.CompilerServices;

//namespace Open3dmm.Graphics
//{
//    using static NativeAbstraction;
//    public unsafe class BrWorldRenderer
//    {
//        private Matrix projection;
//        private Matrix view;
//        public BWLD world;
//        public BasicEffect BasicEffect { get; }

//        public BrWorldRenderer(BWLD world)
//        {
//            this.world = world;
//            BasicEffect = new BasicEffect(GraphicsDevice);
//        }

//        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
//        public void Render()
//        {
//            if (world == null)
//                return;
//            var viewport = GraphicsDevice.Viewport;
//            var vp = new Rectangle(48, 100,
//                                  world.Width1,
//                                  world.Height1);
//            GraphicsDevice.Viewport = new Viewport(vp);
//            GraphicsDevice.BlendState = BlendState.Opaque;
//            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
//            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
//            SetupCamera(world.Camera);
//            SetupLights(world.World);

//            BasicEffect.World = Matrix.Identity;
//            BasicEffect.View = view;
//            BasicEffect.Projection = projection;
//            BasicEffect.EnableDefaultLighting();
//            BasicEffect.TextureEnabled = false;
//            RenderNode(world.World, Matrix.Identity);

//            GraphicsDevice.Viewport = viewport;
//        }

//        private bool SetupCamera(BrActor node)
//        {
//            if (node.Type != BrActorTypes.BR_ACTOR_CAMERA)
//                return false;
//            var cam = (BrCamera*)node.TypeData;
//            this.projection = Matrix.CreatePerspectiveFieldOfView(Microsoft.Xna.Framework.MathHelper.ToRadians(cam->FieldOfView.ToDegrees().ToFloat()), cam->Aspect.ToFloat(), cam->Near.ToFloat(), cam->Far.ToFloat());
//            view = Matrix.Invert(MathHelper.XNAMatrixFromBrMatrix34(&node.Transform.Matrix));
//            return true;
//        }

//        private void SetupLights(BrActor node)
//        {
//            if (node.Type == BrActorTypes.BR_ACTOR_LIGHT)
//            {
//                var light = *(BrLight*)node.TypeData;
//                // TODO: Implement lights
//            }

//            SetupLights(*node.FirstChild);
//            SetupLights(*node.Next);
//        }

//        private void RenderNode(BrActor node, Matrix world)
//        {
//            var newWorld = MathHelper.XNAMatrixFromBrMatrix34(&node.Transform.Matrix) * world;

//            if (node.Type == BrActorTypes.BR_ACTOR_MODEL && node.Model != null)
//            {
//                RenderModel(node, *node.Model, *node.Material, newWorld, node.RenderStyle);
//            }

//            if (node.FirstChild != null)
//                RenderNode(*node.FirstChild, newWorld);
//            if (node.Next != null)
//                RenderNode(*node.Next, world);
//        }

//        private void RenderModel(BrActor node, BrModel model, BrMaterial material, Matrix world, BrRenderStyles renderStyle)
//        {
//            // TODO: Use renderStyle to affect the way we render
//            BasicEffect.World = world;
//            BasicEffect.DiffuseColor = Color.White.ToVector3(); // RandomColor().ToVector3();
//            var m = ResolveModel(model);
//            foreach (var p in BasicEffect.CurrentTechnique.Passes)
//            {
//                p.Apply();
//                m.Render(model);
//            }
//        }

//        private static MODL ResolveModel(BrModel model)
//        {
//            return NativeObject.FromPointer<MODL>(new IntPtr(*(void**)model.Identifier));
//        }
//    }
//}
