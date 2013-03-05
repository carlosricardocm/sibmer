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
    class Misil : Modelo
    {
        public float rapidez;
        public int ddisparo;

        public Misil(float rapidez, int ddisparo, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(modelo, posicion, direccion, PM, VM)
        {
            this.rapidez = rapidez;
            this.ddisparo = ddisparo;
        }

        public void Reset(Vector3 pos, Vector3 dir)
        {
            this.posicion = pos;
            this.direccion = dir;
            this.active = true;
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

        public void Update(Vector3 centropantalla, Vector3 posblanco)
        {
            if (this.active)
            {
                if (this.posicion.Y > posblanco.Y) this.direccion = posblanco - this.posicion;
                //if (this.posicion.Y <= posblanco.Y) this.direccion = this.posicion - posblanco;

                if (this.direccion.Y >= 0) this.direccion.Y = -1;
                this.direccion.Normalize();

                posicion += direccion * this.rapidez;

                RotacionZ = VectorToAngle(this.direccion);
            }
            if (this.active &&
                (posicion.X >= centropantalla.X +GameConstants.PlayfieldSizeX +100 ||
                posicion.X <= centropantalla.X-GameConstants.PlayfieldSizeX -100 ||
                posicion.Y > centropantalla.Y + GameConstants.PlayfieldSizeY / 2 ||
                posicion.Y < centropantalla.Y - GameConstants.PlayfieldSizeY / 2))
                active = false;
        }
    }
}
