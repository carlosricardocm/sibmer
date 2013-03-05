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
    public class Modelo
    {
        public Model modelo;
        public Vector3 posicion;
        public Vector3 direccion;
        public float escala;
        public Matrix MatRot, MatEsc;
        protected float rotacionz, rotaciony, rotacionx;
        public Matrix[] Transformaciones;
        public bool active;

        public Modelo(Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
        {
            this.active = true;
            this.modelo = modelo;
            this.posicion = posicion;
            this.direccion = direccion;
            this.MatEsc = Matrix.CreateScale(1);
            this.MatRot = Matrix.CreateRotationX(MathHelper.PiOver2);
            SetupEffectDefaults(PM, VM);
        }

        public static Vector3 AngleToVector(float angle)
        {
            return new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);
        }

        public static float VectorToAngle(Vector3 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X) - MathHelper.PiOver2;
        }

        public void SetupEffectDefaults(Matrix PM, Matrix VM)
        {
            Transformaciones = new Matrix[modelo.Bones.Count];
            modelo.CopyAbsoluteBoneTransformsTo(Transformaciones);

            foreach (ModelMesh mesh in modelo.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = PM;
                    effect.View = VM;
                }
            }
        }

        public float Escala
        {
            get { return this.escala; }
            set
            {
                escala = value;
                MatEsc = Matrix.CreateScale(escala);
            }
        }

        public virtual float RotacionZ
        {
            get { return rotacionz; }
            set
            {
                rotacionz = value;
                MatRot = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(rotacionz);
            }
        }

        public virtual float RotacionY
        {
            get { return rotaciony; }
            set
            {
                rotaciony = value;
                MatRot = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationY(rotaciony);
            }
        }

        public virtual float RotacionX
        {
            get { return rotacionx; }
            set
            {
                rotaciony = value;
                MatRot = Matrix.CreateRotationX(rotacionx);
            }
        }

        public bool Colision(float scl1, float scl2, Modelo mod2)
        {
            if (this.active && mod2.active)
            {
                BoundingSphere bsmod1 = new BoundingSphere(this.posicion, this.modelo.Meshes[0].BoundingSphere.Radius * scl1);
                BoundingSphere bsmod2 = new BoundingSphere(mod2.posicion, mod2.modelo.Meshes[0].BoundingSphere.Radius * scl2);
                return bsmod1.Intersects(bsmod2);
            }
            return false;
        }

        public virtual void Alpha(float alpha)
        {
            foreach (ModelMesh mesh in modelo.Meshes)
            {
                foreach (BasicEffect efectos in mesh.Effects)
                {
                    efectos.Alpha = alpha;
                }
            }
        }

        public virtual void Draw(Matrix VM)
        {
            if (this.active)
            {
                Matrix TM = this.MatEsc * this.MatRot * Matrix.CreateTranslation(this.posicion);
                //Matrix TM = this.MatRot * Matrix.CreateTranslation(this.posicion);
                foreach (ModelMesh mesh in modelo.Meshes)
                {
                    foreach (BasicEffect efectos in mesh.Effects)
                    {
                        efectos.EnableDefaultLighting();
                        efectos.World = Transformaciones[mesh.ParentBone.Index] * TM;
                        efectos.View = VM;
                    }
                    mesh.Draw();
                }
            }
        }

        public virtual void Draw(Matrix VM, float alpha)
        {
            if (!this.active)
            {
                Matrix TM = this.MatEsc * this.MatRot * Matrix.CreateTranslation(this.posicion);
                //Matrix TM = this.MatRot * Matrix.CreateTranslation(this.posicion);
                foreach (ModelMesh mesh in modelo.Meshes)
                {
                    foreach (BasicEffect efectos in mesh.Effects)
                    {
                        efectos.EnableDefaultLighting();
                        efectos.World = Transformaciones[mesh.ParentBone.Index] * TM;
                        efectos.View = VM;
                        efectos.Alpha = alpha;
                    }
                    mesh.Draw();
                }
            }
        }

        public virtual void Draw(Matrix VM, Vector3 color, float escala)
        {
            if (this.active)
            {
                Matrix TM = this.MatEsc * this.MatRot * Matrix.CreateTranslation(this.posicion);
                //Matrix TM = this.MatRot * Matrix.CreateTranslation(this.posicion);
                foreach (ModelMesh mesh in modelo.Meshes)
                {
                    foreach (BasicEffect efectos in mesh.Effects)
                    {
                        efectos.EnableDefaultLighting();
                        efectos.World = Transformaciones[mesh.ParentBone.Index] * TM * Matrix.CreateScale(escala);
                        efectos.View = VM;

                        efectos.DiffuseColor = color;
                    }
                    mesh.Draw();
                }
            }
        }

        public virtual void DrawActiveOverride(Matrix VM, float alpha)
        {
            Matrix TM = this.MatEsc * this.MatRot * Matrix.CreateTranslation(this.posicion);
            //Matrix TM = this.MatRot * Matrix.CreateTranslation(this.posicion);
            foreach (ModelMesh mesh in modelo.Meshes)
            {
                foreach (BasicEffect efectos in mesh.Effects)
                {
                    efectos.EnableDefaultLighting();
                    efectos.World = Transformaciones[mesh.ParentBone.Index] * TM;
                    efectos.View = VM;
                    efectos.Alpha = alpha;
                }
                mesh.Draw();
            }

        }
    }
}
