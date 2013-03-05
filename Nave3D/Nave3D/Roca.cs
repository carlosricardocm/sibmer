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
    class Roca : Modelo
    {
        public float rapidez;
        public float velgirox, velgiroy, velgiroz;
        public float signo, ejerot;
        Random random;

        public Roca(int rndm, float rapidez, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(modelo, posicion, direccion, PM, VM)
        {
            random = new Random(rndm);
            this.signo = (float)((random.NextDouble() > 0.5) ? 1 : -1);
            this.ejerot = (float)random.NextDouble() * 3;
            this.rapidez = rapidez;
            this.velgiroz = (float)random.NextDouble() * 0.1f;
            this.velgiroy = (float)random.NextDouble() * 0.1f;
            this.velgirox = (float)random.NextDouble() * 0.1f;
        }

        public override float RotacionZ
        {
            get { return rotacionz; }
            set
            {
                rotacionz = value;
                MatRot = Matrix.CreateRotationZ(rotacionz);
            }
        }

        public override float RotacionY
        {
            get { return rotaciony; }
            set
            {
                rotaciony = value;
                MatRot = Matrix.CreateRotationY(rotaciony);
            }
        }

        public override float RotacionX
        {
            get { return rotacionx; }
            set
            {
                rotaciony = value;
                MatRot = Matrix.CreateRotationX(rotacionx);
            }
        }

        public void Update(Vector3 centropantalla)
        {
            if (this.ejerot <= 1) this.RotacionY += this.signo * velgiroy;
            if (this.ejerot <= 2 && this.ejerot > 1) this.RotacionZ += this.signo * velgiroz;
            if (this.ejerot <= 3 && this.ejerot > 2) this.RotacionX += this.signo * velgirox;


            this.posicion += this.direccion * rapidez;

            if (this.posicion.X > (centropantalla.X + GameConstants.PlayfieldSizeX))
                this.posicion.X = (centropantalla.X - GameConstants.PlayfieldSizeX);
            if (this.posicion.X < (centropantalla.X - GameConstants.PlayfieldSizeX))
                this.posicion.X = (centropantalla.X + GameConstants.PlayfieldSizeX);
            if (this.posicion.Y > (centropantalla.Y + GameConstants.PlayfieldSizeY / 2))
                this.posicion.Y = (centropantalla.Y - GameConstants.PlayfieldSizeY / 2);
            if (this.posicion.Y < (centropantalla.Y - GameConstants.PlayfieldSizeY / 2))
                this.posicion.Y = (centropantalla.Y + GameConstants.PlayfieldSizeY / 2);
        }
    }
}
