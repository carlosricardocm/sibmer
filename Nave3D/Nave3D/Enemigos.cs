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
    class Enemigos
    {
        List<Model> tiposbala;
        List<Model> tiposnave;
        List<Model> tiposmisil;

        public Enemigos(List<Model> tiposbala, List<Model> tiposnave, List<Model> tiposmisil)
        {
            this.tiposbala = tiposbala;
            this.tiposnave = tiposnave;
            this.tiposmisil = tiposmisil;
        }

        public void SetPrimeraRonda(ref List<NEnemigo> enemigos, Matrix PM, Matrix VM, Vector3 posinicio, int tb, int tn, int tm)
        {
            if (this.tiposbala.Count > 0 && this.tiposnave.Count > 0 && this.tiposmisil.Count > 0 &&
                tm < this.tiposmisil.Count && tb < this.tiposbala.Count && tn < this.tiposnave.Count)
            {
                Vector3 direccion1 = new Vector3(1, -1, 0);
                direccion1.Normalize();
                Vector3 direccion2 = direccion1;
                direccion2.X *= -1;
                enemigos.Add(new NEnemigo(GameConstants.VidaEnemigo1, GameConstants.RapidezEnemigo1, GameConstants.RapidezBala1, GameConstants.RapidezMisil1, 
                    800, GameConstants.DanoBala1, 0, this.tiposmisil[tm], this.tiposbala[tb], this.tiposnave[tn],
                    new Vector3(posinicio.X - GameConstants.PlayfieldSizeX + 100, posinicio.Y + GameConstants.PlayfieldSizeY/2, posinicio.Z),
                    direccion1, PM, VM));
                enemigos.Add(new NEnemigo(GameConstants.VidaEnemigo1, GameConstants.RapidezEnemigo1, GameConstants.RapidezBala1, GameConstants.RapidezMisil1,
                    800, GameConstants.DanoBala1, 0, this.tiposmisil[tm], this.tiposbala[tb], this.tiposnave[tn],
                    new Vector3(posinicio.X - GameConstants.PlayfieldSizeX + 100, posinicio.Y + GameConstants.PlayfieldSizeY/2 + 4500, posinicio.Z),
                    direccion1, PM, VM));
                enemigos.Add(new NEnemigo(GameConstants.VidaEnemigo1, GameConstants.RapidezEnemigo1, GameConstants.RapidezBala1, GameConstants.RapidezMisil1,
                    800, GameConstants.DanoBala1, 0, this.tiposmisil[tm], this.tiposbala[tb], this.tiposnave[tn],
                    new Vector3(posinicio.X + GameConstants.PlayfieldSizeX - 100, posinicio.Y + GameConstants.PlayfieldSizeY/2, posinicio.Z),
                    direccion2, PM, VM));
                enemigos.Add(new NEnemigo(GameConstants.VidaEnemigo1, GameConstants.RapidezEnemigo1, GameConstants.RapidezBala1, GameConstants.RapidezMisil1,
                    800, GameConstants.DanoBala1, 0, this.tiposmisil[tm], this.tiposbala[tb], this.tiposnave[tn],
                    new Vector3(posinicio.X + GameConstants.PlayfieldSizeX - 100, posinicio.Y + GameConstants.PlayfieldSizeY/2 + 4500, posinicio.Z),
                    direccion2, PM, VM));
            }
        }

        public void SetSegundaRonda(ref List<NEnemigo> enemigos, Matrix PM, Matrix VM, Vector3 posinicio, int tb, int tn, int tm)
        {
            if (this.tiposbala.Count > 0 && this.tiposnave.Count > 0 && this.tiposmisil.Count > 0 &&
                tm < this.tiposmisil.Count && tb < this.tiposbala.Count && tn < this.tiposnave.Count)
            {
                Vector3 direccion = Vector3.Down;
                float xinicio = posinicio.X - GameConstants.PlayfieldSizeX;
                float xinc = GameConstants.PlayfieldSizeX / 2;
                float yinicio = posinicio.Y + GameConstants.PlayfieldSizeY * 2 / 3;
                for (int i = 0; i < 5; i++)
                {
                    enemigos.Add(new NEnemigo(GameConstants.VidaEnemigo1, GameConstants.RapidezEnemigo1, GameConstants.RapidezBala1, GameConstants.RapidezMisil1,
                    800, GameConstants.DanoBala1, 0, this.tiposmisil[tm], this.tiposbala[tb], this.tiposnave[tn],
                    new Vector3(xinicio + i*xinc, yinicio, posinicio.Z),
                    direccion, PM, VM));
                }
            }
        }

        public void SetTerceraRonda(ref List<NEnemigo> enemigos, Matrix PM, Matrix VM, Vector3 posinicio, int tb, int tn, int tm)
        {
            if (this.tiposbala.Count > 0 && this.tiposnave.Count > 0 && this.tiposmisil.Count > 0 &&
                tm < this.tiposmisil.Count && tb < this.tiposbala.Count && tn < this.tiposnave.Count)
            {
                Vector3 direccion = Vector3.Right;
                float yinicio = posinicio.Y - GameConstants.PlayfieldSizeY / 4;
                float yinc = 3500;
                float offsetx = -GameConstants.PlayfieldSizeX;
                for (int i = 0; i < 6; i++)
                {
                    enemigos.Add(new NEnemigo(GameConstants.VidaEnemigo1, GameConstants.RapidezEnemigo1, GameConstants.RapidezBala1, GameConstants.RapidezMisil1,
                    800, GameConstants.DanoBala1, 0, this.tiposmisil[tm], this.tiposbala[tb], this.tiposnave[tn],
                    new Vector3(posinicio.X + offsetx, yinicio + (i*yinc), posinicio.Z),
                    direccion, PM, VM));
                    direccion.X *= -1;
                    offsetx *= -1;
                }

            }
        }

        public void SetPrimerJefe(ref List<Jefe> jefes, Matrix PM, Matrix VM, Vector3 posinicio, int tb, int tn, int tm)
        {
            if (this.tiposbala.Count > 0 && this.tiposnave.Count > 0 && this.tiposmisil.Count > 0 &&
                tm < this.tiposmisil.Count && tb < this.tiposbala.Count && tn < this.tiposnave.Count)
            {
                Vector3 posjefe = posinicio + new Vector3(0, GameConstants.PlayfieldSizeY*2/3, 0);
                Vector3 direccion = Vector3.Down;
                jefes.Add(new Jefe(GameConstants.VidaJefe1, GameConstants.RapidezEnemigo1, GameConstants.RapidezBala1, GameConstants.RapidezMisil1, 
                    800, GameConstants.DanoBala2, 2, tiposmisil[tm], tiposbala[tb], tiposnave[tn], posjefe, direccion, PM, VM));
            }
        }
    }
}
