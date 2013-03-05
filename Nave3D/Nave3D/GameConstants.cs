//-----------------------------------------------------------------------------
// GameConstants.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Nave3D
{
    static class GameConstants
    {
        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 13600.0f;
        public const float PlayfieldSizeY = 18000.0f;
        //asteroid constants
        public const int NumAsteroids = 4;
        public const float AsteroidMinSpeed = 100.0f;
        public const float AsteroidMaxSpeed = 300.0f;
        public const float AsteroidSpeedAdjustment = 5.0f;
        //collision constants
        public const float AsteroidBoundingSphereScale = 0.95f;  //95% size
        public const float ShipBoundingSphereScale = 0.5f;  //50% size
        //bullet constants
        public const int NumBullets = 30;
        public const float BulletSpeedAdjustment = 100.0f;
        //scoring constants
        public const int ShotPenalty = 1;
        public const int DeathPenalty = 50;
        public const int WarpPenalty = 50;
        public const int KillBonus = 25;
        public const int GateBonus = 50;
        public const int RockPenalty = 20;
        //velocidades
        public const float RapidezNave = 15.0f;
        public const float RapidezDesplazamiento = 180;
        public const float RapidezEnemigo1 = 20.0f;
        public const float RapidezBala1 = 100.0f;
        public const float RapidezMisil1 = 100.0f;
        public const float RapidezRoca = 30.0f;
        public const float RapidezPortal = 15.0f;
        public const float RapidezGiroPortal = 0.005f;
        public const float RapidezGiroItem = 0.025f;
        public const float RapidezCambioColor = 0.05f;
        //daños
        public const int DanoBala1 = 1;
        public const int DanoBala2 = 3;
        public const int DanoBala3 = 5;
        public const int DanoMisil1 = 3;
        public const int DanoMisil2 = 5;
        public const int DanoMisil3 = 10;
        public const int DanoNave = 3;
        public const int DanoJefe = 5;
        public const int DanoRoca = 1;
        //vidas
        public const int VidaNave1 = 15;
        public const int VidaEnemigo1 = 1;
        public const int VidaEnemigo2 = 3;
        public const int VidaEnemigo3 = 5;
        public const int VidaJefe1 = 20;
        public const float PorcentajeRespawn = 0.66f;
        //tiempos
        public const double TiempoTotal = 5000;
        public const double TiempoIncremento = 30000;
        public const double TiempoRespawn = 3000;
        public const double TiempoDescripcion = 4000;
        public const double TiempoAparicionItem = 15000;
        public const double TiempoPremiado = 1000;
        public const double TiempoDañado = 1500;
        //Hud
        public const int AlturaMarcadorVida = 40;
        public const float HudWidthPercentage = 0.2f;
        //Umbrales
        public const float UmbralMovimientoKinect = 1000;
        public const float UmbralCentro = 0.07f;
        //Margenes
        public const float MargenResultados = 20;
        public const float MargenAlturaInicialResultados = 100;
        public const float MargenHud = 10;
    }
}
