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
using Microsoft.Kinect;

namespace Nave3D
{
    class Nave : Modelo
    {
        public float rapidez;
        public float rapdisparo;
        private float facdesp;
        public List<Bala> balas;
        public Model bmodelo;
        public int vida;
        public int maxvida;
        public int ddisparo;
        public int ddelay;
        private Vector3 dirb;
        protected Stopwatch sw;
        public bool nuevodisparo;
        private Skeleton skeleton;
        public int x, y;


        public Stopwatch SW
        {
            get { return sw; }
        }

        public Nave(int vida, float rapidez, float rapdisparo, int ddelay, int ddisparo, Model bmodelo, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(modelo, posicion, direccion, PM, VM)
        {
            this.maxvida = vida;
            this.vida = vida;
            this.rapidez = rapidez;
            this.balas = new List<Bala>();
            this.bmodelo = bmodelo;
            this.dirb = Vector3.Up;
            this.RotacionY = 0;
            this.ddelay = ddelay;
            this.ddisparo = ddisparo;
            this.rapdisparo = rapdisparo;
            this.nuevodisparo = false;
            this.facdesp = GameConstants.RapidezDesplazamiento / rapidez;
            //RotacionZ = VectorToAngle(direccion);
            RotacionZ = 0;
            this.sw = new Stopwatch();
            this.sw.Start();
        }

        public void Reset(Vector3 centropantalla)
        {
            this.active = true;
            this.posicion = centropantalla;
            this.direccion = Vector3.Zero;
            this.dirb = Vector3.Up;
            this.RotacionZ = 0;
            this.RotacionY = 0;
            this.vida = (int)(0.7f * this.maxvida);
            this.nuevodisparo = false;
            this.sw = new Stopwatch();
            this.sw.Start();
        }

        public bool ColisionDisparoRoca(ref List<Roca> lista, float scl)
        {
            for (int i = lista.Count - 1; i >= 0; i--)
            {
                for (int j = this.balas.Count - 1; j >= 0; j--)
                {
                    Modelo temp = lista.ElementAt(i);
                    if (this.balas.ElementAt(j).Colision(0.95f, scl, temp))
                    {
                        this.balas.RemoveAt(j);
                        lista.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionNaveRoca(ref List<Roca> lista, float scl)
        {
            if (this.active)
            {
                for (int i = lista.Count - 1; i >= 0; i--)
                {
                    Modelo temp = lista.ElementAt(i);
                    if (this.Colision(0.5f, scl, temp))
                    {
                        this.active = false;
                        lista.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionNaveDisparo(ref List<Bala> bls, float scl)
        {
            if (this.active)
            {
                for (int i = bls.Count - 1; i >= 0; i--)
                {
                    Modelo temp = bls.ElementAt(i);
                    if (this.Colision(0.50f, scl, temp))
                    {
                        this.vida -= bls.ElementAt(i).ddisparo;
                        bls.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionDisparoNave(ref List<NEnemigo> nvs, float scl)
        {
            for (int i = nvs.Count - 1; i >= 0; i--)
            {
                for (int j = this.balas.Count - 1; j >= 0; j--)
                {
                    Modelo temp = nvs.ElementAt(i);
                    if (this.balas.ElementAt(j).Colision(0.95f, scl, temp))
                    {
                        nvs.ElementAt(i).vida -= this.balas.ElementAt(j).ddisparo;
                        this.balas.RemoveAt(j);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionDisparoJefe(ref List<Jefe> jfs, float scl)
        {
            for (int i = jfs.Count - 1; i >= 0; i--)
            {
                for (int j = this.balas.Count - 1; j >= 0; j--)
                {
                    Modelo temp = jfs.ElementAt(i);
                    if (this.balas.ElementAt(j).Colision(0.95f, scl, temp))
                    {
                        jfs.ElementAt(i).vida -= this.balas.ElementAt(j).ddisparo;
                        this.balas.RemoveAt(j);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionDisparoMisil(ref List<Misil> msls, float scl)
        {
            for (int i = msls.Count - 1; i >= 0; i--)
            {
                for (int j = this.balas.Count - 1; j >= 0; j--)
                {
                    Modelo temp = msls.ElementAt(i);
                    if (this.balas.ElementAt(j).Colision(0.95f, scl, temp))
                    {
                        this.balas.RemoveAt(j);
                        msls.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionNaveMisil(ref List<Misil> msls, float scl)
        {
            if (this.active)
            {
                for (int i = msls.Count - 1; i >= 0; i--)
                {
                    Modelo temp = msls.ElementAt(i);
                    if (this.Colision(0.50f, scl, temp))
                    {
                        this.vida -= msls.ElementAt(i).ddisparo;
                        msls.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionNaveNave(ref List<NEnemigo> nvs, float scl)
        {
            if (this.active)
            {
                for (int i = nvs.Count - 1; i >= 0; i--)
                {
                    Modelo temp = nvs.ElementAt(i);
                    if (this.Colision(0.50f, scl, temp))
                    {
                        this.vida -= GameConstants.DanoNave;
                        nvs.ElementAt(i).vida -= GameConstants.DanoNave;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ColisionNaveJefe(ref List<Jefe> jfs, float scl)
        {
            if (this.active)
            {
                for (int i = jfs.Count - 1; i >= 0; i--)
                {
                    Modelo temp = jfs.ElementAt(i);
                    if (this.Colision(0.50f, scl, temp))
                    {
                        this.vida -= GameConstants.DanoJefe;
                        jfs.ElementAt(i).vida -= GameConstants.DanoNave;
                        return true;
                    }
                }
            }
            return false;
        }


        public void CopySkeleton(Skeleton sourceSkeleton)
        {
            if (null == sourceSkeleton)
            {
                return;
            }

            if (null == this.skeleton)
            {
                this.skeleton = new Skeleton();
            }

            this.skeleton = sourceSkeleton;
        }



        public void Update(Vector3 centropantalla, Matrix PM, Matrix VM, int screenWidth, int screenHeight, Camara camara)
        {
            this.facdesp = GameConstants.RapidezDesplazamiento / rapidez ;
            this.posicion.Y = camara.CamTar.Y;

            if (skeleton != null)
            {
                this.posicion.Y += this.rapidez;
                if (this.active)
                {
                    if (this.vida > 0)
                    {
                        Joint manoDerecha = skeleton.Joints[JointType.HandLeft];
                        Vector3 vector = new Vector3
                        {
                            X = (manoDerecha.Position.X) * (rapidez * this.facdesp) * (screenWidth / 2),
                            Y = (manoDerecha.Position.Y) * (rapidez * this.facdesp) * (screenHeight / 2) + (int)(Math.Floor(camara.CamPos.Y)) ,
                            Z = 0.0f,
                        };
                        this.posicion = vector;

                        /*if (kstt.IsKeyUp(Keys.Left) && kstt.IsKeyUp(Keys.Right))
                        {
                            this.RotacionY = 0.0f;
                        }*/
                        /*if (this.posicion.X > centropantalla.X - GameConstants.PlayfieldSizeX && manoDerecha.Position.X < 0)
                        {
                            this.posicion.X -= rapidez * this.facdesp;
                            if (this.RotacionY > -0.5f) this.RotacionY -= 0.10f;
                        }
                        else if (this.posicion.X < centropantalla.X + GameConstants.PlayfieldSizeX && manoDerecha.Position.X > 0)
                        {
                            this.posicion.X += rapidez * this.facdesp;
                            if (this.RotacionY < 0.5f) this.RotacionY += 0.10f;
                        }
                        else if (this.posicion.Y < centropantalla.Y + GameConstants.PlayfieldSizeY / 2 && manoDerecha.Position.Y > 0)
                            this.posicion.Y += rapidez * this.facdesp;
                        else if (this.posicion.Y > centropantalla.Y - GameConstants.PlayfieldSizeY / 2 && manoDerecha.Position.Y < 0)
                            this.posicion.Y -= rapidez * this.facdesp;*/
                    }
                    else
                    {
                        this.active = false;
                    }

                }
            }


            /*this.posicion.Y += this.rapidez;
            if (this.active)
            {
                if (this.vida > 0)
                {
                    KeyboardState kstt = Keyboard.GetState();

                    if (kstt.IsKeyDown(Keys.Space))
                    {
                        if (sw.Elapsed.TotalMilliseconds > this.ddelay)
                        {
                            this.balas.Add(new Bala(this.rapdisparo, this.ddisparo, bmodelo, this.posicion, dirb, PM, VM));
                            this.nuevodisparo = true;
                            sw.Restart();
                        }
                    }
                    if (kstt.IsKeyUp(Keys.Left) && kstt.IsKeyUp(Keys.Right))
                    {
                        this.RotacionY = 0.0f;
                    }
                    if (this.posicion.X > centropantalla.X - GameConstants.PlayfieldSizeX && kstt.IsKeyDown(Keys.Left))
                    {
                        this.posicion.X -= rapidez * this.facdesp;
                        if (this.RotacionY > -0.5f) this.RotacionY -= 0.10f;
                    }
                    else if (this.posicion.X < centropantalla.X + GameConstants.PlayfieldSizeX && kstt.IsKeyDown(Keys.Right))
                    {
                        this.posicion.X += rapidez * this.facdesp;
                        if (this.RotacionY < 0.5f) this.RotacionY += 0.10f;
                    }
                    else if (this.posicion.Y < centropantalla.Y + GameConstants.PlayfieldSizeY / 2 && kstt.IsKeyDown(Keys.Up))
                        this.posicion.Y += rapidez * this.facdesp;
                    else if (this.posicion.Y > centropantalla.Y - GameConstants.PlayfieldSizeY / 2 && kstt.IsKeyDown(Keys.Down))
                        this.posicion.Y -= rapidez * this.facdesp;
                }
                else
                {
                    this.active = false;
                }
            }*/

            for (int i = 0; i < balas.Count; i++)
            {
                if (balas.ElementAt(i).active)
                    balas.ElementAt(i).Update(centropantalla);
                else
                    balas.RemoveAt(i);
            }
        }

        public override void Draw(Matrix VM)
        {
            for (int i = 0; i < balas.Count; i++)
            {
                balas.ElementAt(i).Draw(VM);
            }

            base.Draw(VM);
        }




    }
}
