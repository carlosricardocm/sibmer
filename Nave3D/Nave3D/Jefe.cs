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
using System.Diagnostics;

namespace Nave3D
{
    class Jefe : NEnemigo
    {
        private bool derecha;


        public Jefe(int vida, float rapidez, float rapdisparo, float rapmisil, int ddelay, int ddisparo, int nummisiles, Model mmodelo, Model bmodelo, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(vida, rapidez, rapdisparo, rapmisil, ddelay, ddisparo, nummisiles, mmodelo, bmodelo, modelo, posicion, direccion, PM, VM)
        {
            this.derecha = true;
        }

        public void Update1(Vector3 centropantalla, Vector3 posblanco, bool blancoactive, Matrix PM, Matrix VM)
        {
            if (this.active)
            {
                if (this.escudo > 0)
                {
                    this.RotacionZ = VectorToAngle(Vector3.Down);
                    if (this.posicion.Y > centropantalla.Y + GameConstants.PlayfieldSizeY / 2)
                    {
                    }
                    else
                    {
                        if (swm.Elapsed.Milliseconds > this.ddelay && this.misiles.Count < this.nummisiles && blancoactive)
                        {
                            this.misiles.Add(new Misil(this.rapmisil, GameConstants.DanoMisil2, this.mmodelo, this.posicion, this.direccion, PM, VM));
                            this.misiles.ElementAt(this.misiles.Count - 1).RotacionZ += MathHelper.Pi;
                            this.swm.Restart();
                        }
                        
                        if (this.derecha && this.posicion.X < centropantalla.X + GameConstants.PlayfieldSizeX)
                            this.posicion.X += this.rapidez;
                        if (this.posicion.X >= centropantalla.X + GameConstants.PlayfieldSizeX)
                            this.derecha = false;
                        if (!this.derecha && this.posicion.X > centropantalla.X - GameConstants.PlayfieldSizeX)
                            this.posicion.X -= this.rapidez;
                        if (this.posicion.X <= centropantalla.X - GameConstants.PlayfieldSizeX)
                            this.derecha = true;
                        
                        this.posicion.Y = centropantalla.Y + GameConstants.PlayfieldSizeY / 2;
                        
                        if (sw.Elapsed.TotalMilliseconds > this.ddelay)
                        {
                            this.balas.Add(new Bala(GameConstants.RapidezBala1, this.ddisparo, bmodelo, this.posicion, this.direccion, PM, VM));
                            sw.Restart();
                        }
                    }
                }
                else
                {
                    this.active = false;
                }
            }

            for (int i = 0; i < misiles.Count; i++)
            {
                if (misiles.ElementAt(i).active)
                    misiles.ElementAt(i).Update(centropantalla, posblanco);
                else
                    misiles.RemoveAt(i);
            }

            for (int i = 0; i < balas.Count; i++)
            {
                if (balas.ElementAt(i).active)
                    balas.ElementAt(i).Update(centropantalla);
                else
                    balas.RemoveAt(i);
            }
        }

    }
}
