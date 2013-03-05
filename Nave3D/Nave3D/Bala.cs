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
    class Bala : Modelo
    {
        public float rapidez;
        public int ddisparo;

        public Bala(float rapidez, int ddisparo, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(modelo, posicion, direccion, PM, VM)
        {
            this.rapidez = rapidez;
            this.ddisparo = ddisparo;
        }

        public void Update(Vector3 centropantalla)
        {
            if(this.active)
                posicion += direccion * this.rapidez;
            if (this.active &&
                (posicion.X >= centropantalla.X +GameConstants.PlayfieldSizeX +100 ||
                posicion.X <= centropantalla.X-GameConstants.PlayfieldSizeX -100 ||
                posicion.Y > centropantalla.Y+GameConstants.PlayfieldSizeY / 2 ||
                posicion.Y < centropantalla.Y-GameConstants.PlayfieldSizeY / 2))
                active = false;
        }
    }
}
