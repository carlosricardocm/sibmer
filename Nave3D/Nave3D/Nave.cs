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
        public int escudo;
        public int maxescudo;
        public int ddisparo;
        public int ddelay;
        private Vector3 posold;
        private Vector3 dirb;
        protected Stopwatch sw;
        public bool nuevodisparo;
        private Skeleton skeleton;
        public float hombroy, manoy;
        public enum Vertical { Arriba, Abajo, Centro};
        public enum Horizontal { Izquierda, Derecha, Centro };
        public Vertical vert;
        public Horizontal hor;
        private JointType manojoint;

        public Stopwatch SW
        {
            get { return sw; }
        }

        public Nave(JointType manojoint, int escudo, float rapidez, float rapdisparo, int ddelay, int ddisparo, Model bmodelo, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(modelo, posicion, direccion, PM, VM)
        {
            this.manojoint = manojoint;
            this.posold = Vector3.Zero;
            this.maxescudo = escudo;
            this.escudo = escudo;
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
            this.vert = Vertical.Centro;
            this.hor = Horizontal.Centro;
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
            this.escudo = (int)(GameConstants.PorcentajeRespawn * this.maxescudo);
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
                        this.escudo -= GameConstants.DanoRoca;
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
                        this.escudo -= bls.ElementAt(i).ddisparo;
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
                        nvs.ElementAt(i).escudo -= this.balas.ElementAt(j).ddisparo;
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
                        jfs.ElementAt(i).escudo -= this.balas.ElementAt(j).ddisparo;
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
                        this.escudo -= msls.ElementAt(i).ddisparo;
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
                        this.escudo -= GameConstants.DanoNave;
                        nvs.ElementAt(i).escudo -= GameConstants.DanoNave;
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
                        this.escudo -= GameConstants.DanoJefe;
                        jfs.ElementAt(i).escudo -= GameConstants.DanoNave;
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

        public Vector3 posMano = Vector3.Zero;

        public void Update(Camara camara, int screenWidth, int screenHeight)
        {
            this.facdesp = GameConstants.RapidezDesplazamiento / rapidez ;
            this.posicion.Y += this.rapidez;

            if (skeleton != null)
            {
                this.posicion.Y += this.rapidez;
                if (this.active)
                {
                    if (this.escudo > 0)
                    {
                        Joint Mano = skeleton.Joints[this.manojoint];
                        posMano = new Vector3
                        {
                            X = (Mano.Position.X) * (rapidez * this.facdesp) * (screenWidth / 2),
                            Y = (Mano.Position.Y) * (rapidez * this.facdesp) * (screenHeight / 2) + (int)(Math.Floor(camara.CamPos.Y)) ,
                            Z = 0.0f,
                        };
                       
                        this.posicion = posold;

                        if (posMano.X > camara.CamTar.X - GameConstants.PlayfieldSizeX && posMano.X < camara.CamTar.X + GameConstants.PlayfieldSizeX)
                        {
                            this.posicion.X = posMano.X;

                            if (this.posicion.X < camara.CamTar.X && this.RotacionY > -0.5f) 
                                this.RotacionY -= 0.10f;
                            if (this.posicion.X > camara.CamTar.X && this.RotacionY < 0.5f) 
                                this.RotacionY += 0.10f;
                        }

                        if (posMano.Y < camara.CamTar.Y + GameConstants.PlayfieldSizeY / 2 && posMano.Y > camara.CamTar.Y - GameConstants.PlayfieldSizeY / 2)
                        {
                            this.posicion.Y = posMano.Y;
                            this.posicion.Y += this.rapidez;
                        }
                     


                        posold = this.posicion;
                        posold.Y += camara.rapidez;
                    }
                    else
                    {
                        this.active = false;
                    }

                }
            }

            if (this.escudo < 1) this.active = false;

            for (int i = 0; i < balas.Count; i++)
            {
                if (balas.ElementAt(i).active)
                    balas.ElementAt(i).Update(camara.CamTar);
                else
                    balas.RemoveAt(i);
            }
        }

        public override void Draw(Matrix VM)
        {
            if (balas.Count > 0)
                for (int i = 0; i < balas.Count; i++)
                {
                    balas.ElementAt(i).Draw(VM);
                }

            base.Draw(VM);
        }

        public void Draw(Matrix VM, Microsoft.Xna.Framework.Vector4 color)
        {
            if(balas.Count>0)
                for (int i = 0; i < balas.Count; i++)
                {
                    balas.ElementAt(i).Draw(VM);
                }

            base.Alpha(color.W);

            base.Draw(VM, new Vector3(color.X, color.Y, color.Z), 1);
        }

        public void Draw(Matrix VM, Vector3 color)
        {
            if (balas.Count > 0)
                for (int i = 0; i < balas.Count; i++)
                {
                    balas.ElementAt(i).Draw(VM);
                }

            base.Draw(VM, color, 1);
        }

        public override void Draw(Matrix VM, float alpha)
        {
            if (balas.Count > 0)
                for (int i = 0; i < balas.Count; i++)
                {
                    balas.ElementAt(i).Draw(VM);
                }

            base.DrawActiveOverride(VM, alpha);
        }

        public void Update2(Vector3 centropantalla, Matrix PM, Matrix VM, int screenWidth, int screenHeight, Camara camara)
        {
            this.facdesp = GameConstants.RapidezDesplazamiento / rapidez;
            this.posicion.Y += this.rapidez;

            

            if (skeleton != null)
            {
                //this.posicion.Y += this.rapidez;
                if (this.active)
                {
                    if (this.escudo > 0)
                    {
                        Joint manoIzquierda = skeleton.Joints[JointType.HandLeft];
                        Joint hombroIzquierda = skeleton.Joints[JointType.ShoulderLeft];

                       

                        float posXManoIzquierda = manoIzquierda.Position.X;
                        float posYManoIzquierda = manoIzquierda.Position.Y;
                        float posXHombroIzquierda = hombroIzquierda.Position.X;
                        float posYHombroIzquierda = hombroIzquierda.Position.Y;

                        this.hombroy = posYHombroIzquierda;
                        this.manoy = posYManoIzquierda;

                        if (posXManoIzquierda < posXHombroIzquierda - GameConstants.UmbralCentro)
                        {
                            hor = Horizontal.Izquierda;
                        }
                        else if (posXManoIzquierda > posXHombroIzquierda + GameConstants.UmbralCentro)
                        {
                            hor = Horizontal.Derecha;
                        }
                        else
                        {
                            hor = Horizontal.Centro;
                        }

                        if (posYManoIzquierda < posYHombroIzquierda - GameConstants.UmbralCentro)
                        {
                            vert = Vertical.Abajo;
                        }
                        else if (posYManoIzquierda > posYHombroIzquierda + GameConstants.UmbralCentro)
                        {
                            vert = Vertical.Arriba;
                        }
                        else
                        {
                            vert = Vertical.Centro;
                        }




                        if (this.posicion.X > centropantalla.X - GameConstants.PlayfieldSizeX && hor == Horizontal.Izquierda)
                        {
                            this.posicion.X -= rapidez * this.facdesp;
                            if (this.RotacionY > -0.5f) this.RotacionY -= 0.10f;
                        }

                        if (this.posicion.X < centropantalla.X + GameConstants.PlayfieldSizeX && hor == Horizontal.Derecha)
                        {
                            this.posicion.X += rapidez * this.facdesp;
                            if (this.RotacionY < 0.5f) this.RotacionY += 0.10f;
                        }

                        if (this.hor == Horizontal.Centro)
                            this.RotacionY = 0.0f;

                        if (this.posicion.Y < centropantalla.Y + GameConstants.PlayfieldSizeY / 2 && vert == Vertical.Arriba)
                        {
                            this.posicion.Y += rapidez * this.facdesp;
                        }

                        if (this.posicion.Y > centropantalla.Y - GameConstants.PlayfieldSizeY / 2 && vert == Vertical.Abajo)
                        {
                            this.posicion.Y -= rapidez * this.facdesp;
                        }
                     
                    }
                    else
                    {
                        this.active = false;
                    }

                }
            }
                 
            for (int i = 0; i < balas.Count; i++)
            {
                if (balas.ElementAt(i).active)
                    balas.ElementAt(i).Update(centropantalla);
                else
                    balas.RemoveAt(i);
            }
        }

        public void Update3(Camara camara, int screenWidth, int screenHeight)
        {
            this.facdesp = GameConstants.RapidezDesplazamiento / rapidez;
            this.posicion.Y += this.rapidez;

            if (skeleton != null)
            {
                this.posicion.Y += this.rapidez;
                    Joint Mano = skeleton.Joints[this.manojoint];
                    posMano = new Vector3
                    {
                        X = (Mano.Position.X) * (rapidez * this.facdesp) * (screenWidth / 2),
                        Y = (Mano.Position.Y) * (rapidez * this.facdesp) * (screenHeight / 2) + (int)(Math.Floor(camara.CamPos.Y)),
                        Z = 0.0f,
                    };

                    this.posicion = posold;
                    
                    if (posMano.X > camara.CamTar.X - GameConstants.PlayfieldSizeX && posMano.X < camara.CamTar.X + GameConstants.PlayfieldSizeX)
                    {
                        this.posicion.X = posMano.X;

                        if (this.posicion.X < camara.CamTar.X && this.RotacionY > -0.5f)
                            this.RotacionY -= 0.10f;
                        if (this.posicion.X > camara.CamTar.X && this.RotacionY < 0.5f)
                            this.RotacionY += 0.10f;
                    }

                    if (posMano.Y < camara.CamTar.Y + GameConstants.PlayfieldSizeY / 2 && posMano.Y > camara.CamTar.Y - GameConstants.PlayfieldSizeY / 2)
                    {
                        this.posicion.Y = posMano.Y;
                        this.posicion.Y += this.rapidez;
                    }



                    posold = this.posicion;
                    posold.Y += camara.rapidez;
                if(this.escudo < 1)
                {
                    this.active = false;
                }

                
            }

            if (this.escudo < 1) this.active = false;

            for (int i = 0; i < balas.Count; i++)
            {
                if (balas.ElementAt(i).active)
                    balas.ElementAt(i).Update(camara.CamTar);
                else
                    balas.RemoveAt(i);
            }
        }

     }
}
