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
    class Portal : Item
    {
        public float rapidez;
        protected Vector3 statpos;

        public Portal(float rapidez, float rapgiro, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(rapgiro, modelo, posicion, direccion, PM, VM)
        {
            this.rapidez = rapidez;
            this.statpos = posicion;
        }

        override public void Update(Vector3 centropantalla)
        {
            if (this.active)
            {
                //this.posicion += this.direccion * this.rapidez;
                this.posicion = statpos + centropantalla;

                if (this.posicion.X > (centropantalla.X + GameConstants.PlayfieldSizeX))
                    this.posicion.X = (centropantalla.X - GameConstants.PlayfieldSizeX);
                if (this.posicion.X < (centropantalla.X - GameConstants.PlayfieldSizeX))
                    this.posicion.X = (centropantalla.X + GameConstants.PlayfieldSizeX);
                if (this.posicion.Y > (centropantalla.Y + GameConstants.PlayfieldSizeY / 2))
                    this.posicion.Y = (centropantalla.Y - GameConstants.PlayfieldSizeY / 2)+1000;
                if (this.posicion.Y < (centropantalla.Y - GameConstants.PlayfieldSizeY / 2))
                    this.posicion.Y = (centropantalla.Y + GameConstants.PlayfieldSizeY / 2)-1000;

                if (!this.cruzado)
                {
                    this.RotacionY += rapgiro;
                }
                else
                {
                    if (this.rapgiro < 3)
                    {
                        RotacionY -= this.rapgiro;
                        Escala += this.rapgiro;
                        this.rapgiro += 0.15f;
                    }
                    else
                    {
                        this.active = false;
                    }
                }
            }
        }
    }
}
