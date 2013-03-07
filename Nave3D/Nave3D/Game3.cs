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
using MySql.Data.MySqlClient;
using Microsoft.Kinect;
using System.Diagnostics;
using RehabFramework;

namespace Nave3D
{

    public class Game3 : Microsoft.Xna.Framework.Game
    {
        #region Declaracion de variables
        string[] args;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GamePadState lastState = GamePad.GetState(PlayerIndex.One);
        KeyboardState lkstt = Keyboard.GetState(PlayerIndex.One);

        Mensaje mensaje;

        //variables para la conexion con la base de datos
        MySqlConnection conection = new MySqlConnection();
        string cadenaConexion;
        bool existesesion = false;

        //variable stop wacth
        Stopwatch sw;

        //estado de la nave
        enum estadoNave { premiado, dañado, normal};
        estadoNave edonave = estadoNave.normal;
        float tcolornave = 0;
        float swen = 0;
        bool tcninc = true;
        Texture2D caritaFeliz, caritaTriste, caritaSeria;
            
        //Tiempo de juego
        Stopwatch juegotimer;
        double TiempoExtra;

        //Camera/View information
        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
        Vector3 Target = Vector3.Zero;
        float aspectRatio;
        Camara camara;
        float width, height;

        //Game States
        enum GameState { Activo, Descripcion, Resultados };
        GameState CGS;

        //View Ports
        Viewport PantallaCompleta;
        Viewport Juego;
        Viewport Hud;

        //Audio components
        AudioEngine audioEngine;
        WaveBank waveBank;
        SoundBank soundBank;
        //SoundEffect gyges;
        //SoundEffectInstance gygesInstance;
        // Cue so we can hang on to the sound of the engine.
        Cue engineSound = null;
        Cue backgroundMusic = null;

        //Visual components
        Random random = new Random();
        Texture2D stars;
        Texture2D Descripcion;
        Texture2D TexBlanca;
        Rectangle texcoord;
        SpriteFont lucidaConsole;
        Nave nave;
        List<Roca> rocas;
        List<Item> items;
        List<Portal> gates;
        List<Vector3> posgts;
        List<Vector2> gtsindex;
        int gtset;
        int gti;
        bool inverso = false;
        Portal gate;
        Enemigos enemigos;
        List<NEnemigo> listaenemigos;
        List<Jefe> listajefes;
        float camrap;

        //Hud
        Texture2D HudTex;
        Texture2D ShieldTex;
        Texture2D ManoDer;
        Texture2D ManoIzq;
        SpriteFont HudFont;
        bool shldlphup;
        bool tiempoup;
        float ShieldAlpha;
        float TiempoAlpha;
        double TInicial;
        int score;
        Vector2 HudPos = new Vector2(GameConstants.MargenHud);
        int PortalesCreados;
        int PortalesCruzados;
        int Respawns;

        //Resultados
        SpriteFont ResFont;
        CuadroDeTexto TBObservaciones;
        float resscl=1;
        SpriteFont ComFont;
        float resmrgn=GameConstants.MargenResultados;
        Vector2 ResPos = new Vector2(GameConstants.MargenResultados, GameConstants.MargenAlturaInicialResultados);
        Boton BtnTerminar;

        //Kinect Components
        private readonly KinectChooser chooser;
        private static Skeleton[] SkeletonData { get; set; }
        private SkeletonFrame skeletonFrame;
        private bool skeletonDetected;

        string idSesion;
        JointType Mano;
        string manocad = "";
        //Fin Kinect Components

        bool pause;
        #endregion

        public Game3()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.chooser = new KinectChooser(this, ColorImageFormat.RgbResolution640x480Fps30, DepthImageFormat.Resolution640x480Fps30);
            try
            {
                if (this.chooser.Sensor != null) this.chooser.Sensor.ElevationAngle = 0;
            }
            catch (Exception ex)
            {

            }
            this.Services.AddService(typeof(KinectChooser), this.chooser);
            this.Components.Add(this.chooser);
            aspectRatio = (float)GraphicsDeviceManager.DefaultBackBufferWidth * 0.8f  / GraphicsDeviceManager.DefaultBackBufferHeight;
        }

        protected override void Initialize()
        {
            //inicializacion variable stop watch
            sw= new Stopwatch() ;
            //sw.Start();

            juegotimer = new Stopwatch();
            //juegotimer.Start();
            TiempoExtra = 0;

            CGS = GameState.Descripcion;

            audioEngine = new AudioEngine("Content\\Audio\\MyGameAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");
           
            //gyges = Content.Load<SoundEffect>("Audio\\Waves\\gyges");
            //gygesInstance = gyges.CreateInstance();

            //this.graphics.IsFullScreen = true;
            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = (int)width;
            graphics.PreferredBackBufferHeight = (int)height;

            this.graphics.ToggleFullScreen();
            camrap = 5.0f;

            this.TexBlanca = new Texture2D(GraphicsDevice, 1, 1);
            this.TexBlanca.SetData(new Color[] { Color.White });

            texcoord = new Rectangle(0, 0, (int)width, (int)height);

            PantallaCompleta = graphics.GraphicsDevice.Viewport;
            Juego = PantallaCompleta;
            Hud = PantallaCompleta;
            Juego.Width = (int)(width * (1-GameConstants.HudWidthPercentage));
            Hud.Width = (int)(width * GameConstants.HudWidthPercentage);
            Hud.X = Juego.Width;

            camara = new Camara(this, cameraPosition, Vector3.Zero, Vector3.Up, aspectRatio, GameConstants.RapidezNave);
            Components.Add(camara);
            this.camara.pause = true;

            listaenemigos = new List<NEnemigo>();
            listajefes = new List<Jefe>();

            ShieldAlpha = 1;
            shldlphup = false;

            PortalesCreados = 0;
            PortalesCruzados = 0;
            Respawns = 0;

            base.Initialize();
        }

        List<Roca> ResetRocas()
        {
            List<Roca> rcs = new List<Roca>();
            float xStart;
            float yStart;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)(camara.CamTar.X-GameConstants.PlayfieldSizeX);
                }
                else
                {
                    xStart = (float)(camara.CamTar.X+GameConstants.PlayfieldSizeX);
                }
                yStart = (float)random.NextDouble() * (camara.CamTar.Y+GameConstants.PlayfieldSizeY -1000);
                double angle;
                do
                {
                    angle = random.NextDouble() * 2 * Math.PI;
                }
                while (angle == Math.PI/2 || angle == Math.PI);

                rcs.Add(new Roca(i, GameConstants.RapidezRoca,
                Content.Load<Model>("Models/asteroid1"), new Vector3(xStart, yStart, 0.0f), 
                new Vector3(-(float)Math.Sin(angle), (float)Math.Cos(angle), 0), camara.Proyeccion, camara.Vista));
                if (i > 0 && rcs[rcs.Count - 1].ejerot == rcs[rcs.Count - 2].ejerot) 
                {
                    int temper;
                    do
                    {
                        temper = (int)random.NextDouble() * 3;
                    }while ((int)temper == (int)rcs[rcs.Count - 2].ejerot);
                    rcs[rcs.Count-1].ejerot = temper;
                }
            }

            return rcs;
        }

        void RefillRocas(ref List<Roca> rcs)
        {
            int i = rcs.Count;
            float xStart;
            float yStart;
            while (rcs.Count < GameConstants.NumAsteroids)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)(camara.CamTar.X - GameConstants.PlayfieldSizeX);
                }
                else
                {
                    xStart = (float)(camara.CamTar.X + GameConstants.PlayfieldSizeX);
                }
                yStart = (float)random.NextDouble() * (camara.CamTar.Y + GameConstants.PlayfieldSizeY - 1000);
                double angle;
                do
                {
                    angle = random.NextDouble() * 2 * Math.PI;
                }
                while (angle == Math.PI / 2 || angle == Math.PI);

                rcs.Add(new Roca(i, GameConstants.RapidezRoca,
                Content.Load<Model>("Models/asteroid1"), new Vector3(xStart, yStart, 0.0f),
                new Vector3(-(float)Math.Sin(angle), (float)Math.Cos(angle), 0), camara.Proyeccion, camara.Vista));
                if (i > 0 && rcs[rcs.Count - 1].ejerot == rcs[rcs.Count - 2].ejerot)
                {
                    int temper;
                    do
                    {
                        temper = (int)random.NextDouble() * 3;
                    } while ((int)temper == (int)rcs[rcs.Count - 2].ejerot);
                    rcs[rcs.Count - 1].ejerot = temper;
                }
            }
        }

        List<Portal> SetGates()
        {
            float xStart;
            float yStart;
            List<Portal> gts = new List<Portal>();
            int nportales = 0;

            if (this.juegotimer.ElapsedMilliseconds <= TInicial / 3) nportales = 1;
            if (this.juegotimer.ElapsedMilliseconds > TInicial / 3 &&
                this.juegotimer.ElapsedMilliseconds <= TInicial * 2 / 3) nportales = 2;
            if (this.juegotimer.ElapsedMilliseconds > TInicial * 2 / 3) nportales = 3;

            for (int i = 0; i < nportales; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)(camara.CamTar.X - GameConstants.PlayfieldSizeX);
                }
                else
                {
                    xStart = (float)(camara.CamTar.X + GameConstants.PlayfieldSizeX);
                }
                yStart = (float)random.Next(i) * (camara.CamTar.Y + GameConstants.PlayfieldSizeY - 1000);

                double angle = 0;
                
                do
                {
                    angle = random.NextDouble() * 2 * Math.PI;
                }
                while (angle == Math.PI / 2 || angle == Math.PI || angle == Math.PI*3/2 || angle == 0 || angle == Math.PI*2);

                gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal, 
                    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0.0f), 
                    new Vector3(-(float)Math.Sin(angle), (float)Math.Cos(angle), 0), camara.Proyeccion, camara.Vista));
                PortalesCreados++;
            }

            return gts;
        }

        List<Portal> SetGates2()
        {
            float xStart;
            float yStart;
            List<Portal> gts = new List<Portal>();
            int nportales = 0;

            if (this.juegotimer.ElapsedMilliseconds <= TInicial / 3) nportales = 1;
            if (this.juegotimer.ElapsedMilliseconds > TInicial / 3 &&
                this.juegotimer.ElapsedMilliseconds <= TInicial * 2 / 3) nportales = 2;
            if (this.juegotimer.ElapsedMilliseconds > TInicial * 2 / 3) nportales = 3;

            for (int i = 0; i < nportales; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)(camara.CamTar.X - GameConstants.PlayfieldSizeX);
                }
                else
                {
                    xStart = (float)(camara.CamTar.X + GameConstants.PlayfieldSizeX);
                }
                yStart = (float)random.Next(i) * (camara.CamTar.Y + GameConstants.PlayfieldSizeY - 1000);

                double angle = 0;
                
                do
                {
                    angle = random.NextDouble() * 2 * Math.PI;
                }
                while (angle == Math.PI / 2 || angle == Math.PI || angle == Math.PI*3/2 || angle == 0 || angle == Math.PI*2);

                gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal, 
                    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0.0f),
                    new Vector3(-(float)Math.Sin(angle), (float)Math.Cos(angle), 0), camara.Proyeccion, camara.Vista));
                    //new Vector3(-(float)Math.Sin(angle), (float)Math.Cos(angle), 0), camara.Proyeccion, camara.Vista));
                PortalesCreados++;
            }

            return gts;
        }

        void FillGates(/*ref List<Portal> gts*/)
        {
            float ancho = GameConstants.PlayfieldSizeX * 0.95f;
            float alto = GameConstants.PlayfieldSizeY / 2 * 0.95f;
            float p;
            int divisiones = 4;
            int pdiv = 8;
            //gts = new List<Portal>();
            posgts = new List<Vector3>();
            gtsindex = new List<Vector2>();
            int tini;

            tini = 0;
            float xStart = 0;
            float yStart = alto;
            //vertical centrada
            for (int i = 0; i <= divisiones; i++)
            {
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                yStart -= alto * 2 / divisiones;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            xStart = -ancho;
            yStart = 0;
            //horizontal centrada
            for (int i = 0; i <= divisiones; i++)
            {
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                xStart += ancho * 2 / divisiones;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            xStart = -ancho;
            yStart = alto;
            //diagonal invertida
            for (int i = 0; i <= divisiones; i++)
            {
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                yStart -= alto * 2 / divisiones;
                xStart += ancho * 2 / divisiones;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            xStart = -ancho;
            yStart = -alto;
            //diagonal
            for (int i = 0; i <= divisiones; i++)
            {
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                yStart += alto * 2 / divisiones;
                xStart += ancho * 2 / divisiones;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            xStart = -ancho;
            p = (ancho * ancho) / (4 * alto);
            //parabola hacia abajo
            for (int i = 0; i <= pdiv; i++)
            {
                yStart = (xStart * xStart) / (4 * p);
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                xStart += ancho * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            xStart = -ancho;
            //parabola hacia arriba
            for (int i = 0; i <= pdiv; i++)
            {
                yStart = -(xStart * xStart) / (4 * p);
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                xStart += ancho * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            xStart = -ancho;
            //N invertida
            for (int i = 0; i <= pdiv; i++)
            {
                yStart = (xStart < 0) ? ((xStart * xStart) / (4 * p)) : (-(xStart * xStart) / (4 * p));
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                xStart += ancho * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            xStart = ancho;
            //N
            for (int i = 0; i <= pdiv; i++)
            {
                yStart = (xStart > 0) ? ((xStart * xStart) / (4 * p)) : (-(xStart * xStart) / (4 * p));
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                xStart -= ancho * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            yStart = alto;
            p = (alto * alto) / (4 * ancho);
            //parabola hacia la izquierda
            for (int i = 0; i <= pdiv; i++)
            {
                xStart = (yStart * yStart) / (4 * p);
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                yStart -= alto * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            yStart = alto;
            //parabola hacia la derecha
            for (int i = 0; i <= pdiv; i++)
            {
                xStart = -(yStart * yStart) / (4 * p);
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                yStart -= alto * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            yStart = alto;
            //Z
            for (int i = 0; i <= pdiv; i++)
            {
                xStart = (yStart > 0) ? (-(yStart * yStart) / (4 * p)) : ((yStart * yStart) / (4 * p));
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                yStart -= alto * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

            tini = posgts.Count;
            yStart = -alto;
            //S
            for (int i = 0; i <= pdiv; i++)
            {
                xStart = (yStart < 0) ? (-(yStart * yStart) / (4 * p)) : ((yStart * yStart) / (4 * p));
                //gts.Add(new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                //    Content.Load<Model>("Models/portal"), new Vector3(xStart, yStart, 0), new Vector3(0), camara.Proyeccion, camara.Vista));
                posgts.Add(new Vector3(xStart, yStart, 0));
                yStart += alto * 2 / pdiv;
            }
            gtsindex.Add(new Vector2(tini, posgts.Count - 1));

        }

        List<Item> SetItems(float posicion)
        {
            List<Item> itms = new List<Item>();
            float posx, rapgiro = GameConstants.RapidezGiroItem;
                posx = ((float)random.NextDouble() * GameConstants.PlayfieldSizeX)-GameConstants.PlayfieldSizeX/2;
                itms.Add(new Item(rapgiro, Content.Load<Model>("Models/TimeBonus"), 
                    new Vector3(posx, posicion,0),Vector3.Up, camara.Proyeccion, camara.Vista));
            return itms;
        }

        List<Item> SetItems(List<Vector3> posiciones)
        {
            List<Item> itms = new List<Item>();
            float rapgiro = 0.005f;

            for (int i = 0; i < posiciones.Count; i++)
            {
                itms.Add(new Item(rapgiro, Content.Load<Model>("Models/portal"), posiciones[i], Vector3.Zero, camara.Proyeccion, camara.Vista));
            }

            return itms;
        }

        public static bool RandomBool()
        {
            return new Random().Next(100) % 2 == 0;
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            stars = Content.Load<Texture2D>("Textures/bg01");
            Descripcion = Content.Load<Texture2D>("Textures/Descripcion2");
            HudTex = Content.Load<Texture2D>("Textures/HudTex");
            ShieldTex = Content.Load<Texture2D>("Textures/ShieldTex2");
            ManoDer = Content.Load<Texture2D>("Textures/ManoDerecha");
            ManoIzq = Content.Load<Texture2D>("Textures/ManoIzquierda");
            caritaFeliz = Content.Load<Texture2D>("Textures/caritaFeliz");
            caritaSeria = Content.Load<Texture2D>("Textures/caritaSeria");
            caritaTriste = Content.Load<Texture2D>("Textures/caritaTriste");
            int tempid;

            lucidaConsole = Content.Load<SpriteFont>("Fonts/Lucida Console");
            HudFont = Content.Load<SpriteFont>("Fonts/HudFont");
            ResFont = Content.Load<SpriteFont>("Fonts/ResFont");
            ComFont = Content.Load<SpriteFont>("Fonts/ComFont");

            if (this.args.Count() > 0)
            {
                if (int.TryParse(this.args[0], out tempid))
                {
                    idSesion = this.args[0];
                }
                else
                    idSesion = "not valid";
            }
            else
                idSesion = "not valid";
            if (this.args.Count() > 1)
            {
                if (this.args[1].Equals("Izquierda"))
                {
                    this.Mano = JointType.HandLeft;
                    this.manocad = "Izquierda";
                }
                else if (this.args[1].Equals("Derecha"))
                {
                    this.Mano = JointType.HandRight;
                    this.manocad = "Derecha";
                }
                else
                {
                    this.Mano = JointType.HandLeft;
                    this.manocad = "Izquierda";
                }


            }
            else
            {
                this.Mano = JointType.HandLeft;
                this.manocad = "Izquierda";
            }

            if (this.args.Count() > 2)
            {
                if (Double.TryParse(args[2], out TInicial))
                {
                    TInicial *= 60000;
                }
                else
                {
                    TInicial = GameConstants.TiempoTotal;
                }
            }
            else
            {
                TInicial = GameConstants.TiempoTotal;
            }
            
            nave = new Nave(Mano, GameConstants.VidaNave1, GameConstants.RapidezNave, GameConstants.RapidezBala1, 200, GameConstants.DanoBala1, Content.Load<Model>("Models/pea_proj"), Content.Load<Model>("Models/p1_wedge"), 
                Vector3.Zero, Vector3.Zero, camara.Proyeccion, camara.Vista);
            
            rocas = new List<Roca>();
            RefillRocas(ref rocas);

            items = new List<Item>();

            FillGates();

            gtset = random.Next(gtsindex.Count);
            inverso = RandomBool();
            if (!inverso)
            {
                gti = (int)gtsindex.ElementAt(gtset).X;
            }
            else
            {
                gti = (int)gtsindex.ElementAt(gtset).Y;
            }

            List<Model> bls = new List<Model>();
            bls.Add(Content.Load<Model>("Models/pea_proj"));
            List<Model> nvs = new List<Model>();
            nvs.Add(Content.Load<Model>("Models/p1_wedge"));
            List<Model> msls = new List<Model>();
            msls.Add(Content.Load<Model>("Models/misil"));
            enemigos = new Enemigos(bls,nvs, msls);

            mensaje = new Mensaje(GraphicsDevice, Content, "Fonts", "Textures", (new String[] { "TrianguloAlerta", "TrianguloAlerta", "TrianguloAlerta" }), "TituloRecuadro",
                "DescripcionRecuadro", Color.White, Color.Blue, new Vector2(width / 2 - width / 6, height / 5), width / 3, 10, 64);


            inicializarConeccion();

        }

        protected override void UnloadContent()
        {
        }

        //Kinect Chooser Object
        public KinectChooser Chooser
        {
            get
            {
                return (KinectChooser)this.Services.GetService(typeof(KinectChooser));
            }
        }

        public void RecogeArgumentos(string[] args)
        {
            this.args = args;
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            switch(CGS)
            {
                case GameState.Descripcion:
                    UpdateDescripcion();
                    break;
                case GameState.Activo:
                    UpdateActivo();
                    break;
                case GameState.Resultados:
                    UpdateResultados();
                    break;
            }
            
            base.Update(gameTime);
        }

        #region Metodos para actualizar el juego cuando esta la descripcion
        protected void UpdateDescripcion()
        {
            if (!this.juegotimer.IsRunning) this.juegotimer.Start();
            if (this.juegotimer.ElapsedMilliseconds > GameConstants.TiempoDescripcion)
            {
                this.juegotimer.Stop();
                this.juegotimer.Reset();
                this.CGS = GameState.Activo;
            }
        }
        #endregion

        #region Metodos para actualizar el juego cuando esta activo
        protected void BGM()
        {
            if (backgroundMusic == null)
            {
                backgroundMusic = soundBank.GetCue("gyges");
                backgroundMusic.Play();
            }

            if (backgroundMusic.IsStopped)
            {
                backgroundMusic.Resume();
            }

            if (backgroundMusic.IsPaused)
            {
                backgroundMusic.Resume();
            }
        }

        bool InBounds(int index, Vector2 rango)
        {
            if (index >= rango.X && index <= rango.Y) return true;
            return false;
        }

        bool InBounds(int index, int size)
        {
            if (index >= 0 && index <= size - 1) return true;
            return false;
        }

        void EHVacio(object sender, EventArgs e)
        {

        }

        void EHTerminar(object sender, EventArgs e)
        {
            if (!this.idSesion.Equals("not valid"))
            {
                try
                {

                    cadenaConexion = "Server=127.0.0.1; Database=kinect_rehab; Uid=root; Pwd=lol;";
                    conection.ConnectionString = cadenaConexion;
                    conection.Open();
                }
                catch (MySqlException)
                {
                    mensaje.Show(Content, "ERROR", "Ocurrio un error al intentar conectarse a la base de datos.", Mensaje.Tipo.Notificacion);
                    mensaje.AddWidget(new Boton(GraphicsDevice, Content, "Aceptar", "FuenteSelector", Color.Gray, Color.WhiteSmoke, mensaje.Posicion, 10, AceptarYSalir));
                    return;
                }
                MySqlCommand instruccion = conection.CreateCommand();
                instruccion.CommandText = "INSERT INTO juego(id_sesion, nombre, duracion, observaciones, puntaje, mano) VALUES (" +
                    this.idSesion + ", '" + Process.GetCurrentProcess().ProcessName + "', '" +
                    TimeFormat(((int)(this.juegotimer.ElapsedMilliseconds / 1000)) * 1000, true) + "', '" +
                    this.TBObservaciones.Cadena + "', '" + 
                    this.score + "', '" + 
                    this.manocad + "')";
                instruccion.ExecuteNonQuery();

                conection.Close();
            }

            this.Exit();
        }

        protected void UpdateResultados()
        {
            MouseState msstt = Mouse.GetState();
            if (msstt.LeftButton == ButtonState.Pressed)
            {
                if (Ayuda.SobreRectangulo(msstt, this.TBObservaciones.Posicion, this.TBObservaciones.Width, this.TBObservaciones.Height))
                {
                    this.TBObservaciones.Activo = true;
                }
                else
                {
                    this.TBObservaciones.Activo = false;
                }
            }
            this.TBObservaciones.Update();
            this.BtnTerminar.Posicion = new Vector2(this.BtnTerminar.Posicion.X, this.TBObservaciones.Posicion.Y + this.TBObservaciones.Height);
            this.BtnTerminar.Update(Vector2.Zero);
        }

        protected void UpdateActivo()
        {
            if (!this.juegotimer.IsRunning && this.juegotimer.ElapsedMilliseconds == 0)
            {
                this.juegotimer.Start();
            }
            if (this.chooser.Sensor != null && /*skeletonFrame != null &&*/ !this.juegotimer.IsRunning && 
                this.juegotimer.ElapsedMilliseconds > 0 && this.juegotimer.ElapsedMilliseconds < TInicial + this.TiempoExtra)
            {
                this.juegotimer.Start();
            }
            if (this.juegotimer.IsRunning)
            {

                if (this.juegotimer.ElapsedMilliseconds >= TInicial + this.TiempoExtra)
                {
                    this.juegotimer.Stop();
                    this.camara.pause = true;
                    this.CGS = GameState.Resultados;
                    float cPos = 0;
                    if (cPos < this.ResFont.MeasureString("Tiempo de Juego: " + TimeFormat(((int)(this.juegotimer.ElapsedMilliseconds / 1000)) * 1000)).X + ResPos.X)
                        cPos = this.ResPos.X + this.ResFont.MeasureString("Tiempo de Juego: " + TimeFormat(((int)(this.juegotimer.ElapsedMilliseconds / 1000)) * 1000)).X;
                    if (cPos < this.ResFont.MeasureString("Portales Cruzados: " + PortalesCruzados.ToString()).X + ResPos.X)
                        cPos = this.ResPos.X + this.ResFont.MeasureString("Portales Cruzados: " + PortalesCruzados.ToString()).X;
                    if (cPos < this.ResFont.MeasureString("Naves Perdidas: " + Respawns.ToString()).X + ResPos.X)
                        cPos = this.ResPos.X + this.ResFont.MeasureString("Naves Perdidas: " + Respawns.ToString()).X;
                    if (cPos < this.ResFont.MeasureString("Puntaje Final: " + score.ToString()).X + ResPos.X)
                        cPos = this.ResPos.X + this.ResFont.MeasureString("Puntaje Final: " + score.ToString()).X;
                    float twidth = ResFont.MeasureString("Mano Usada: ").X;
                    if (this.Mano == JointType.HandRight)
                    {
                        float hndscl = (twidth) / this.ManoDer.Width / 2;
                        if (cPos < this.ManoDer.Width * hndscl + ResPos.X) cPos = this.ResPos.X + this.ManoDer.Width * hndscl;
                    }
                    if (this.Mano == JointType.HandLeft)
                    {
                        float hndscl = (twidth) / this.ManoIzq.Width / 2;
                        if (cPos < this.ManoIzq.Width * hndscl + ResPos.X) cPos = this.ResPos.X + this.ManoIzq.Width * hndscl;
                    }

                    resscl = (this.width-resmrgn*2)/2 / cPos;
                    Color cfuente = Color.Yellow;

                    //TBObservaciones = new CuadroDeTexto(this.GraphicsDevice, this.width-resmrgn*2, new Vector2(this.width+resmrgn,this.ResPos.Y), Color.Aquamarine*0.5f, 
                    //    cfuente, ComFont, TexBlanca, "Comentario", CuadroDeTexto.Tipo.Texto, TexBlanca, TexBlanca, 200, EHVacio);

                    //BtnTerminar = new Boton(this.GraphicsDevice, this.Content, "Terminar", "DescripcionRecuadro", Color.Gray,
                    //    Color.WhiteSmoke, new Vector2(this.width + resmrgn, this.TBObservaciones.Posicion.Y + this.TBObservaciones.Height), 10, EHTerminar);

                    TBObservaciones = new CuadroDeTexto(this.GraphicsDevice, this.width/2 - resmrgn * 2, 
                        new Vector2(this.width/2+resmrgn, GameConstants.MargenAlturaInicialResultados), Color.DarkBlue * 0.5f,
                        cfuente, ComFont, TexBlanca, "Comentario", CuadroDeTexto.Tipo.Texto, TexBlanca, TexBlanca, 200, EHVacio);

                    BtnTerminar = new Boton(this.GraphicsDevice, this.Content, "Terminar", "DescripcionRecuadro", Color.Gray,
                        Color.WhiteSmoke, new Vector2(this.width / 2 + resmrgn, this.TBObservaciones.Posicion.Y + this.TBObservaciones.Height), 10, EHTerminar);

                    TBObservaciones.Activo = true;

                    this.IsMouseVisible = true;

                    return;
                }

                Sounds(Keyboard.GetState());

                //float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;


                // Update audioEngine.
                audioEngine.Update();

                UpdateKinect();

                UpdateColisiones();

                if (TInicial + this.TiempoExtra - this.juegotimer.ElapsedMilliseconds <= GameConstants.TiempoAparicionItem && this.items.Count == 0)
                {
                    this.items = SetItems(this.camara.CamTar.Y + GameConstants.PlayfieldSizeY * 2 / 3);
                }

                if (!nave.active && sw.ElapsedMilliseconds >= GameConstants.TiempoRespawn)
                {
                    score -= GameConstants.WarpPenalty;
                    soundBank.PlayCue("hyperspace_activate");
                    nave.Reset(camara.CamTar);
                    sw.Stop();
                    sw.Reset();
                }

                if (!nave.active && sw.ElapsedMilliseconds == 0)
                {
                    this.Respawns++;
                    sw.Start();
                }

                //for (int i = (int)gtsindex.ElementAt(gtset).Y; i >= (int)gtsindex.ElementAt(gtset).X; i--)
                //{
                //    gates[i].Update(camara.CamTar);
                //    if (!gates[i].active) gates.RemoveAt(i);
                //}

                #region seleccion de la linea para los portales y su actualizacion
                if (gate == null || !gate.active)
                {
                    int tempset;
                    if (!inverso && InBounds(gti,gtsindex.ElementAt(gtset)))
                    {
                        gate = new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                            Content.Load<Model>("Models/portal"), posgts.ElementAt(gti), new Vector3(0), camara.Proyeccion, camara.Vista);
                        gti++;
                    }
                    else if (!inverso && !InBounds(gti, gtsindex.ElementAt(gtset)))
                    {
                        tempset = random.Next(0, gtsindex.Count - 1);
                        if (tempset != gtset && InBounds(gtset, gtsindex.Count))
                        {
                            gtset = tempset;
                        }
                        else if (tempset == gtset && InBounds(tempset + 1, gtsindex.Count))
                        {
                            gtset = tempset + 1;
                        }
                        else if (tempset == gtset && InBounds(tempset - 1, gtsindex.Count))
                        {
                            gtset = tempset - 1;
                        }
                        inverso = RandomBool();
                        if (!inverso)
                        {
                            gti = (int)gtsindex.ElementAt(gtset).X;
                        }
                        else
                        {
                            gti = (int)gtsindex.ElementAt(gtset).Y;
                        }
                        gate = new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                            Content.Load<Model>("Models/portal"), posgts.ElementAt(gti), new Vector3(0), camara.Proyeccion, camara.Vista);
                        gti += (inverso) ? -1 : 1;

                    }
                    else if (inverso && InBounds(gti, gtsindex.ElementAt(gtset)))
                    {
                        gate = new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                            Content.Load<Model>("Models/portal"), posgts.ElementAt(gti), new Vector3(0), camara.Proyeccion, camara.Vista);
                        gti--;
                    }
                    else if (inverso && !InBounds(gti, gtsindex.ElementAt(gtset)))
                    {
                        tempset = random.Next(0, gtsindex.Count - 1);
                        if (tempset != gtset && InBounds(gtset, gtsindex.Count))
                        {
                            gtset = tempset;
                        }
                        else if (tempset == gtset && InBounds(tempset + 1, gtsindex.Count))
                        {
                            gtset = tempset + 1;
                        }
                        else if (tempset == gtset && InBounds(tempset - 1, gtsindex.Count))
                        {
                            gtset = tempset - 1;
                        }
                        inverso = RandomBool();
                        if (!inverso)
                        {
                            gti = (int)gtsindex.ElementAt(gtset).X;
                        }
                        else
                        {
                            gti = (int)gtsindex.ElementAt(gtset).Y;
                        }
                        gate = new Portal(GameConstants.RapidezPortal, GameConstants.RapidezGiroPortal,
                            Content.Load<Model>("Models/portal"), posgts.ElementAt(gti), new Vector3(0), camara.Proyeccion, camara.Vista);
                        gti += (inverso) ? -1 : 1;
                    }

                }
                else if (gate != null && gate.active)
                {
                    gate.Update(camara.CamTar);
                }
                #endregion

                for (int i = rocas.Count - 1; i >= 0; i--)
                {
                    rocas[i].Update(camara.CamTar);
                    if (!rocas[i].active) rocas.RemoveAt(i);
                }

                for (int i = items.Count - 1; i >= 0; i--)
                {
                    items[i].Update(camara.CamTar);
                    if (!items[i].active) items.RemoveAt(i);
                }

                nave.Update3(camara, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
                //nave.Update2(camara.CamTar, camara.Proyeccion, camara.Vista, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height, camara);

                switch (edonave)
                {
                    case estadoNave.normal:
                        break;
                    case estadoNave.premiado:
                        if (this.tcninc && this.tcolornave < 1.0f)
                        {
                            this.tcolornave += GameConstants.RapidezCambioColor;
                        }
                        else if (this.tcninc && this.tcolornave >= 1.0f)
                        {
                            this.tcninc = false;
                        }
                        if (!this.tcninc && this.tcolornave > 0.0f)
                        {
                            this.tcolornave -= GameConstants.RapidezCambioColor;
                        }
                        else if (!this.tcninc && this.tcolornave <= 0.0f)
                        {
                            this.tcninc = true;
                        }

                        if (juegotimer.ElapsedMilliseconds-swen > GameConstants.TiempoPremiado)
                        {
                            this.edonave = estadoNave.normal;
                        }
                        break;
                    case estadoNave.dañado:
                        if (this.tcninc && this.tcolornave < 1.0f)
                        {
                            this.tcolornave += GameConstants.RapidezCambioColor;
                        }
                        else if (this.tcninc && this.tcolornave >= 1.0f)
                        {
                            this.tcninc = false;
                        }
                        if (!this.tcninc && this.tcolornave > 0.0f)
                        {
                            this.tcolornave -= GameConstants.RapidezCambioColor;
                        }
                        else if (!this.tcninc && this.tcolornave <= 0.0f)
                        {
                            this.tcninc = true;
                        }

                        if (juegotimer.ElapsedMilliseconds - swen > GameConstants.TiempoDañado)
                        {
                            this.edonave = estadoNave.normal;
                        }
                        break;
                }

                if (rocas.Count < GameConstants.NumAsteroids)
                    RefillRocas(ref rocas);

                //if (gates.Count == 0)
                //    FillGates();

                switch ((int)(camara.CamTar.Y))
                {
                    case (int)GameConstants.RapidezNave * 500:
                        //enemigos.SetPrimeraRonda(ref listaenemigos,camara.Proyeccion, camara.Vista, camara.CamTar, 0, 0, 0);
                        break;
                    case (int)GameConstants.RapidezNave * 1500:
                        listaenemigos.Clear();
                        //enemigos.SetSegundaRonda(ref listaenemigos, camara.Proyeccion, camara.Vista, camara.CamTar, 0, 0, 0);
                        break;
                    case (int)GameConstants.RapidezNave * 2800:
                        listaenemigos.Clear();
                        //enemigos.SetTerceraRonda(ref listaenemigos, camara.Proyeccion, camara.Vista, camara.CamTar, 0, 0, 0);
                        break;
                    case (int)GameConstants.RapidezNave * 4000:
                        listajefes.Clear();
                        listaenemigos.Clear();
                        //enemigos.SetPrimerJefe(ref listajefes, camara.Proyeccion, camara.Vista, camara.CamTar, 0, 0, 0);
                        break;
                }
                texcoord.Y -= 2;
            }
            else
            {
                if (backgroundMusic != null && backgroundMusic.IsPlaying)
                {
                    backgroundMusic.Pause();
                }
            }
        }

        protected void UpdateKinect()
        {
            // If the sensor is not found, not running, or not connected, stop now
            if (null == Chooser.Sensor ||
                false == Chooser.Sensor.IsRunning ||
                KinectStatus.Connected != Chooser.Sensor.Status)
            {
                pause = true;
                camara.pause = true;
                this.juegotimer.Stop();
                return;
            }
            else
            {
                if (this.juegotimer.IsRunning)
                {
                    pause = false;
                    camara.pause = false;
                }
            }


            bool newFrame = false;
            this.Chooser.Sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            using (skeletonFrame = this.Chooser.Sensor.SkeletonStream.OpenNextFrame(0))
            {

                // Sometimes we get a null frame back if no data is ready
                if (null != skeletonFrame)
                {
                    newFrame = true;

                    // Reallocate if necessary
                    if (null == SkeletonData || SkeletonData.Length != skeletonFrame.SkeletonArrayLength)
                    {
                        SkeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    skeletonFrame.CopySkeletonDataTo(SkeletonData);

                    // Select the first tracked skeleton we see 
                    Skeleton rawSkeleton =
                        (from s in SkeletonData
                         where s != null && s.TrackingState == SkeletonTrackingState.Tracked
                         select s).FirstOrDefault();

                    if (null != rawSkeleton)
                    {

                        this.nave.CopySkeleton(rawSkeleton);
                        //this.animator.CopySkeleton(rawSkeleton);
                        //this.animator.FloorClipPlane = skeletonFrame.FloorClipPlane;

                        // Reset the filters if the skeleton was not seen before now
                        //if (this.skeletonDetected == false)
                        //{
                        // this.animator.Reset();
                        //}

                        this.skeletonDetected = true;
                    }
                    else
                    {
                        this.skeletonDetected = false;
                    }
                }
            }
        }

        protected void UpdateColisiones()
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                items[i].Update(camara.CamTar);
                if (nave.active && items[i].Colision(0.65f, 1.0f, nave))
                {
                    if (!items[i].cruzado) this.TiempoExtra += GameConstants.TiempoIncremento;
                    items[i].cruzado = true;
                    soundBank.PlayCue("hyperspace_activate");
                }
            }

            //for (int i = gates.Count - 1; i >= 0; i--)
            //{
            //    gates[i].Update(camara.CamTar);
            //    if (nave.active && gates[i].Colision(0.75f, 1.3f, nave))
            //    {
            //        if (!gates[i].cruzado)
            //        {
            //            this.score += GameConstants.GateBonus;
            //            PortalesCruzados++;
            //        }
            //        gates[i].cruzado = true;
            //        soundBank.PlayCue("hyperspace_activate");
            //    }
            //}

            if (nave.active && gate != null && gate.Colision(0.75f, 1.3f, nave))
            {
                if (!gate.cruzado)
                {
                    this.score += GameConstants.GateBonus;
                    PortalesCruzados++;
                    this.edonave = estadoNave.premiado;
                    this.swen = this.juegotimer.ElapsedMilliseconds;
                }
                gate.cruzado = true;
                soundBank.PlayCue("hyperspace_activate");
            }

            for (int i = listaenemigos.Count - 1; i >= 0; i--)
            {
                listaenemigos[i].Update(camara.CamTar, nave.posicion, nave.active, camara.Proyeccion, camara.Vista);
                if (nave.ColisionNaveDisparo(ref listaenemigos[i].balas, 0.95f))
                {
                    soundBank.PlayCue("explosion2");
                }

                if (!listaenemigos[i].active && listaenemigos[i].misiles.Count == 0 && listaenemigos[i].balas.Count == 0) listaenemigos.RemoveAt(i);
            }

            for (int i = listajefes.Count - 1; i >= 0; i--)
            {
                listajefes[i].Update1(camara.CamTar, nave.posicion, nave.active, camara.Proyeccion, camara.Vista);

                if (nave.ColisionNaveDisparo(ref listajefes[i].balas, 0.95f))
                {
                    soundBank.PlayCue("explosion2");
                }
                if (nave.ColisionNaveMisil(ref listajefes[i].misiles, 0.10f))
                {
                    soundBank.PlayCue("explosion2");
                }

                if (nave.ColisionDisparoMisil(ref listajefes[i].misiles, 0.50f))
                {
                    soundBank.PlayCue("explosion2");
                }

                if (!listajefes[i].active && listajefes[i].misiles.Count == 0 && listajefes[i].balas.Count == 0) listajefes.RemoveAt(i);
            }

            if (nave.active && nave.ColisionNaveJefe(ref listajefes, 0.5f))
            {
                soundBank.PlayCue("explosion3");
            }

            if (nave.ColisionDisparoJefe(ref listajefes, 0.5f))
            {
                soundBank.PlayCue("explosion2");
                score += GameConstants.KillBonus;
            }

            if (nave.active && nave.ColisionNaveNave(ref listaenemigos, 0.5f))
            {
                soundBank.PlayCue("explosion3");
            }

            if (nave.ColisionDisparoNave(ref listaenemigos, 0.5f))
            {
                soundBank.PlayCue("explosion2");
                score += GameConstants.KillBonus;
            }

            if (nave.ColisionDisparoRoca(ref rocas, 0.67f))
            {
                soundBank.PlayCue("explosion2");
                score += GameConstants.KillBonus;
            }

            if (nave.active && nave.ColisionNaveRoca(ref rocas, 0.67f))
            {
                soundBank.PlayCue("explosion3");
                score -= GameConstants.RockPenalty;
                this.edonave = estadoNave.dañado;
                this.swen = this.juegotimer.ElapsedMilliseconds;
            }
        }

        protected void Sounds(KeyboardState kstt)
        {
            BGM();

            if (nave.active && kstt.IsKeyDown(Keys.Up) || kstt.IsKeyDown(Keys.Down) || kstt.IsKeyDown(Keys.Left) || kstt.IsKeyDown(Keys.Right))
            {
                if (engineSound == null)
                {
                    engineSound = soundBank.GetCue("engine_2");
                    engineSound.Play();
                }
                else if (engineSound.IsPaused)
                {
                    engineSound.Resume();
                }
            }
            else
            {
                if (engineSound != null && engineSound.IsPlaying)
                {
                    engineSound.Pause();
                }

            }

            if (nave.active && kstt.IsKeyDown(Keys.Space) && nave.nuevodisparo)
            {
                score -= GameConstants.ShotPenalty;
                soundBank.PlayCue("tx0_fire1");
                nave.nuevodisparo = false;
            }

            lkstt = kstt;
        }
        #endregion

        void AceptarYSalir(object sender, EventArgs e)
        {
            //DateTime duracionSesion;

            mensaje.Activo = false;
            //if (SesionActual != null)
            //{
            //    SesionActual.horaFin = DateTime.Now.ToString("HH:mm:ss");
            //    //duracionSesion = Convert.ToDateTime("06/09/2005 " + SesionActual.duracionTotal) + (Convert.ToDateTime("06/09/2005 " + SesionActual.horaFin) - Convert.ToDateTime("06/09/2005 " + SesionActual.horaInicio));
            //    //SesionActual.duracionTotal = duracionSesion.ToString("HH:mm:ss");
            //    MySqlCommand instruccion = new MySqlCommand();
            //    instruccion = conection.CreateCommand();
            //    instruccion.CommandText = "UPDATE sesion SET sesion.hora_fin = '" + SesionActual.horaFin + "' WHERE sesion._id = " + SesionActual._id + ";";

            //    instruccion.ExecuteNonQuery();

            //}

            this.Exit();

        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            switch(CGS)
            {
                case GameState.Descripcion:
                    DrawDescripcion();
                    break;
                case GameState.Activo:
                    DrawActivo();
                    break;
                case GameState.Resultados:
                    DrawResultados();
                    break;
            }
            
            base.Draw(gameTime);
            
        }

        #region Metodos para dibujar el juego cuando esta en descripcion
        protected void DrawDescripcion()
        {
            graphics.GraphicsDevice.Viewport = PantallaCompleta;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(Descripcion, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);
            spriteBatch.End();
        }
        #endregion

        #region Metodos para Dibujar el juego cuando esta activo
        protected void DrawActivo()
        {
            DrawJuego();
            DrawHud();
        }

        protected void DrawJuego()
        {
            graphics.GraphicsDevice.Viewport = Juego;

            //fondo
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, null, null);
            spriteBatch.Draw(stars, Vector2.Zero, texcoord, Color.White);
            spriteBatch.End();

            if (pause != true)
            {
                //nave con balas
                if (nave.active)
                {
                    switch (this.edonave)
                    {
                        case estadoNave.normal:
                            nave.Draw(camara.Vista, Color.White.ToVector3());
                            break;
                        case estadoNave.premiado:
                            nave.Draw(camara.Vista, Color.Lerp(Color.White, Color.Purple, this.tcolornave).ToVector3());
                            break;
                        case estadoNave.dañado:
                            nave.Draw(camara.Vista, Color.Lerp(Color.White, Color.Red, this.tcolornave).ToVector3());
                            break;
                    }
                }
                else
                {
                    nave.Draw(camara.Vista, 0.3f);
                }

                //rocas
                for (int i = 0; i < rocas.Count; i++)
                {
                    rocas[i].Draw(camara.Vista);
                }

                //items
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].Draw(camara.Vista);
                }

                //portales
                //for (int i = (int)gtsindex.ElementAt(gtset).X; i <= (int)gtsindex.ElementAt(gtset).Y; i++)
                //{
                //    gates[i].Draw(camara.Vista);
                //}
                if(gate != null)gate.Draw(camara.Vista);

                //enemigos
                if (listaenemigos.Count > 0)
                    for (int i = 0; i < listaenemigos.Count; i++)
                    {
                        listaenemigos[i].Draw(camara.Vista);
                    }
                if (listajefes.Count > 0)
                    for (int i = 0; i < listajefes.Count; i++)
                    {
                        listajefes[i].Draw(camara.Vista);
                    }
            }
        }

        protected void DrawHud()
        {

            graphics.GraphicsDevice.Viewport = Hud;

            Vector2 tPos = HudPos;

            Color cfuente = Color.Yellow;

            //marcador
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(HudTex, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);

            spriteBatch.DrawString(HudFont, "Puntaje: ", tPos, cfuente);
            tPos.Y += HudFont.MeasureString("Puntaje: ").Y;
            spriteBatch.DrawString(HudFont, score.ToString(), tPos, cfuente);
            tPos.Y += HudFont.MeasureString(score.ToString()).Y;

            spriteBatch.DrawString(HudFont, "Escudo: ", tPos, cfuente);
            tPos.Y += HudFont.MeasureString("Escudo: ").Y;
            tPos.Y += DrawShield1(tPos);

            if (TInicial + this.TiempoExtra - this.juegotimer.ElapsedMilliseconds <= 5000)
            {
                if (!this.tiempoup && this.TiempoAlpha > 0)
                {
                    this.TiempoAlpha -= 0.05f;
                }
                else
                {
                    this.tiempoup = true;
                }

                if (this.tiempoup && this.TiempoAlpha < 1)
                {
                    this.TiempoAlpha += 0.05f;
                }
                else
                {
                    this.tiempoup = false;
                }
            }
            else
            {
                this.TiempoAlpha = 1;
                this.tiempoup = false;
            }

            spriteBatch.DrawString(HudFont, "Tiempo \nRestante: ", tPos, cfuente);
            tPos.Y += HudFont.MeasureString("Tiempo \nRestante: ").Y;
            spriteBatch.DrawString(HudFont, TimeFormat(TInicial + this.TiempoExtra - this.juegotimer.ElapsedMilliseconds), tPos, Color.Lerp(Color.Red,cfuente * this.TiempoAlpha, this.TiempoAlpha));
            tPos.Y += HudFont.MeasureString(TimeFormat(TInicial + this.TiempoExtra - this.juegotimer.ElapsedMilliseconds)).Y;

            float twidth = width / (1 / GameConstants.HudWidthPercentage);

            if (this.Mano == JointType.HandRight)
            {
                spriteBatch.DrawString(HudFont, "Mano: ", tPos, cfuente);
                tPos.Y += HudFont.MeasureString("Mano: ").Y;

                float hndscl = (twidth - 20) / this.ManoDer.Width/2;

                spriteBatch.Draw(this.ManoDer, tPos, null, Color.White, 0f, Vector2.Zero, hndscl, SpriteEffects.None, 0f);

                tPos.Y += this.ManoDer.Height * hndscl;
            }

            if (this.Mano == JointType.HandLeft)
            {
                spriteBatch.DrawString(HudFont, "Mano: ", tPos, cfuente);
                tPos.Y += HudFont.MeasureString("Mano: ").Y;

                float hndscl = (twidth - 20) / this.ManoIzq.Width / 2;

                spriteBatch.Draw(this.ManoIzq, tPos, null, Color.White, 0f, Vector2.Zero, hndscl, SpriteEffects.None, 0f);

                tPos.Y += this.ManoIzq.Height * hndscl;
            }
            float escala = ((int)(width * GameConstants.HudWidthPercentage) - GameConstants.MargenHud * 2 < this.height - tPos.Y - GameConstants.MargenHud) ?
                (int)(width * GameConstants.HudWidthPercentage) - GameConstants.MargenHud * 2 : this.height - tPos.Y - GameConstants.MargenHud;

            switch (this.edonave)
            {
                case estadoNave.normal:
                    spriteBatch.Draw(caritaSeria, new Rectangle((int)tPos.X, (int)tPos.Y, (int)escala, (int)escala), Color.White);
                    break;
                case estadoNave.premiado:
                    spriteBatch.Draw(caritaFeliz, new Rectangle((int)tPos.X, (int)tPos.Y, (int)escala, (int)escala), Color.White);
                    break;
                case estadoNave.dañado:
                    spriteBatch.Draw(caritaTriste, new Rectangle((int)tPos.X, (int)tPos.Y, (int)escala, (int)escala), Color.White);
                    break;
            }

            spriteBatch.End();
            graphics.GraphicsDevice.Viewport = PantallaCompleta;

        }

        protected string TimeFormat(double msegundos, bool fbase)
        {
            double segundos = msegundos / 1000.0f;

            string cad = "";

            int thor = (int)(segundos / 3600);
            if (thor > 0 || fbase)
            {
                if (thor < 10)
                    cad += "0";
                cad += thor.ToString() + ":";
            }
            int tmin = (int)((segundos - thor * 3600) / 60);
            if (tmin < 10)
                cad += "0";
            cad += tmin.ToString() + ":";
            int tsec = (int)(segundos - thor * 3600 - tmin * 60);
            int msc = (int)((segundos - thor * 3600 - tmin * 60 - tsec) * 1000);
            if (tsec < 10)
                cad += "0";
            if (msc < 0) msc = 0;
            cad += tsec.ToString();
            if (!fbase) cad += "." + msc;

            return cad;
        }

        protected string TimeFormat(double msegundos)
        {
            double segundos = msegundos/1000.0f;

            string cad = "";
            
            int thor = (int)(segundos/3600);
            if (thor > 0)
            {
                if (thor < 10)
                    cad += "0";
                cad += thor.ToString() + ":";
            }
            int tmin = (int)((segundos-thor*3600)/60);
                if (tmin < 10)
                    cad += "0";
                cad += tmin.ToString() + ":";
            int tsec = (int)(segundos-thor*3600-tmin*60);
            int msc = (int)((segundos-thor*3600-tmin*60-tsec)*1000);
            if(tsec<10)
                cad += "0";
            if (msc < 0) msc = 0;
            cad += tsec.ToString() + "." + msc;

            return cad;
        }

        protected float DrawShield1(Vector2 Pos)
        {
            graphics.GraphicsDevice.Viewport = Hud;

            float twidth = width / (1/GameConstants.HudWidthPercentage);
            float theight = height;

            int maxshld = this.nave.maxescudo;
            int shldstrngth = this.nave.escudo;

            Vector2 tPos = Pos;
            int recw = (int)((twidth - 10) / maxshld);
            int rech = (int)((twidth - 10) / 5);

            for (int i = 0; i < shldstrngth; i++)
            {
                Color cshld = Color.Lerp(Color.Red, Color.Green, ((float)(i + maxshld * 0.35f) / (float)maxshld));

                spriteBatch.Draw(this.TexBlanca, tPos, null, cshld, 0f, Vector2.Zero, new Vector2(recw - 1, rech), SpriteEffects.None, 0f);
                tPos.X += recw;
            }
            tPos.Y += rech;

            return rech;
        }

        protected float DrawShield2(Vector2 Pos)
        {
            graphics.GraphicsDevice.Viewport = Hud;

            float twidth = width / (1/GameConstants.HudWidthPercentage);
            float theight = height;

            int maxshld = this.nave.maxescudo;
            int shldstrngth = this.nave.escudo;

            float shldscl = (twidth-20) / this.ShieldTex.Width;

            Color cshld = Color.Lerp(Color.Red, Color.Green, ((float)(shldstrngth) / (float)maxshld));

            if (shldstrngth <= maxshld / 3)
            {
                if (!this.shldlphup && this.ShieldAlpha > 0)
                {
                    this.ShieldAlpha -= 0.05f;
                }
                else
                {
                    this.shldlphup = true;
                }

                if (this.shldlphup && this.ShieldAlpha < 1)
                {
                    this.ShieldAlpha += 0.05f;
                }
                else
                {
                    this.shldlphup = false;
                }

                cshld *= ShieldAlpha;
            }

            spriteBatch.Draw(this.ShieldTex, Pos, null, cshld, 0f, Vector2.Zero, shldscl, SpriteEffects.None, 0f);

            return this.ShieldTex.Height*shldscl;
        }
        #endregion

        #region Metodos para dibujar el juego cuando esta en resultados
        protected void DrawResultados()
        {
            Vector2 tPos = ResPos;

            Color cfuente = Color.Yellow;

            graphics.GraphicsDevice.Viewport = PantallaCompleta;

            //fondo
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, null, null);
            spriteBatch.Draw(stars, Vector2.Zero, texcoord, Color.White);
            spriteBatch.End();

            //Resultados
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.DrawString(ResFont, "Tiempo de Juego: "+TimeFormat(((int)(this.juegotimer.ElapsedMilliseconds/1000))*1000, true), tPos, cfuente,
                0,Vector2.Zero, new Vector2(resscl,1), SpriteEffects.None, 0);
            tPos.Y += ResFont.MeasureString("Tiempo de Juego: "+TimeFormat(((int)(this.juegotimer.ElapsedMilliseconds/1000))*1000, true)).Y*1.5f;

            spriteBatch.DrawString(ResFont, "Portales Cruzados: " + PortalesCruzados.ToString(), tPos, cfuente,
                0, Vector2.Zero, new Vector2(resscl, 1), SpriteEffects.None, 0);
            tPos.Y += ResFont.MeasureString("Portales Cruzados: " + PortalesCruzados.ToString()).Y * 1.5f;

            spriteBatch.DrawString(ResFont, "Naves Perdidas: " + Respawns.ToString(), tPos, cfuente,
                0, Vector2.Zero, new Vector2(resscl, 1), SpriteEffects.None, 0);
            tPos.Y += ResFont.MeasureString("Naves Perdidas: " + Respawns.ToString()).Y * 1.5f;

            spriteBatch.DrawString(ResFont, "Puntaje Final: " + score.ToString(), tPos, cfuente,
                0, Vector2.Zero, new Vector2(resscl, 1), SpriteEffects.None, 0);
            tPos.Y += ResFont.MeasureString("Puntaje Final: " + score.ToString()).Y * 1.5f;

            spriteBatch.DrawString(ResFont, "Mano Usada: ", tPos, cfuente,
                0, Vector2.Zero, new Vector2(resscl, 1), SpriteEffects.None, 0);
            tPos.X += ResFont.MeasureString("Mano Usada: ").X;


            float twidth = tPos.X-ResPos.X;

            if (this.Mano == JointType.HandRight)
            {
                float hndscl = (twidth) / this.ManoDer.Width / 2;

                spriteBatch.Draw(this.ManoDer, tPos, null, Color.White, 0f, Vector2.Zero, hndscl, SpriteEffects.None, 0f);
            }

            if (this.Mano == JointType.HandLeft)
            {
                float hndscl = (twidth) / this.ManoIzq.Width / 2;

                spriteBatch.Draw(this.ManoIzq, tPos, null, Color.White, 0f, Vector2.Zero, hndscl, SpriteEffects.None, 0f);
            }

            this.TBObservaciones.Draw(spriteBatch, Vector2.Zero);
            this.BtnTerminar.Draw(spriteBatch, Vector2.Zero);

            spriteBatch.End();
        }
        #endregion

        void inicializarConeccion()
        {
            //seccion donde se leen los datos para el terapueta
            if (!this.idSesion.Equals("not valid"))
            {
                try
                {

                    cadenaConexion = "Server=127.0.0.1; Database=kinect_rehab; Uid=root; Pwd=lol;";
                    //cadenaConexion = "Server=127.0.0.1; Database=kinect_rehab; Uid=root; Pwd=blaziken1005;";
                    //cadenaConexion = "Server = 127.0.0.1; Database = materias; Uid = root; Pwd = lol; CharSet = UTF8;";
                    conection.ConnectionString = cadenaConexion;
                    conection.Open();
                    //MessageBox.Show("¡¡¡La conexion se ha realizado exitosamente!!!", "¡¡¡Bien Hecho!!!");
                }
                catch (MySqlException)
                //catch()
                {
                    //MessageBox.Show("Ocurrio un error al intentar conectarse.", "ERROR");
                    mensaje.Show(Content, "ERROR", "Ocurrio un error al intentar conectarse a la base de datos.", Mensaje.Tipo.Notificacion);
                    mensaje.AddWidget(new Boton(GraphicsDevice, Content, "Aceptar", "FuenteSelector", Color.Gray, Color.WhiteSmoke, mensaje.Posicion, 10, AceptarYSalir));
                    return;
                }
                MySqlCommand instruccion = conection.CreateCommand();
                instruccion.CommandText = "SELECT * FROM sesion WHERE _id = "+this.idSesion;
                MySqlDataReader reader = instruccion.ExecuteReader();

                if (reader.HasRows)
                {
                    this.existesesion = true;
                }
                else
                {
                    this.idSesion = "not valid";
                }
                reader.Close();
                conection.Close();
            }

        }
    }
}
