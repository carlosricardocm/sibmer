using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Nave3D
{
    public class Camara : Microsoft.Xna.Framework.GameComponent
    {
        //Camera matrices
        private Matrix vista;
        private Matrix proyeccion;

        public Vector3 CamPos;
        public Vector3 CamTar;
        public Vector3 CamUp;

        public bool pause;

        public float rapidez, delta;

        public Matrix Vista
        {
            get { return this.vista; }
        }

        public Matrix Proyeccion
        {
            get { return this.proyeccion; }
        }

        public Camara(Game game, Vector3 pos, Vector3 target, Vector3 up, float aspectratio, float rapidez)
            :base(game)
        {
            this.pause = false;
            this.rapidez = rapidez;
            this.CamPos = pos;
            this.CamTar = target;
            this.CamUp = up;
            CreateLookAt();

            proyeccion = Matrix.CreatePerspectiveFieldOfView(
              MathHelper.ToRadians(45.0f),
              aspectratio,
              GameConstants.CameraHeight - 15000.0f,
              GameConstants.CameraHeight + 15000.0f);
        }
        
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (!pause)
            {

                CamPos.Y += rapidez;
                CamTar.Y += rapidez;
                CreateLookAt();

            }
                base.Update(gameTime);
        }

        private void CreateLookAt()
        {
            vista = Matrix.CreateLookAt(CamPos, CamTar, CamUp);
        }

    }

}
