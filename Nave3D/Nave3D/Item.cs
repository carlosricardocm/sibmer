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
    class Item : Modelo
    {
        protected float rapgiro;
        public bool cruzado;

        public Item(float rapgiro, Model modelo, Vector3 posicion, Vector3 direccion, Matrix PM, Matrix VM)
            : base(modelo, posicion, direccion, PM, VM)
        {
            this.rapgiro = rapgiro;
            this.cruzado = false;
        }

        public virtual void Update(Vector3 centropantalla)
        {
            if (this.active)
            {
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
