﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SDL2.SDL;

namespace GOB_Life
{
    enum Gtype
    {
        empty,
        //coddons:
        start,
        stop,
        input,
        output,
        skip,
        undo,
        //operations:
        add,
        sub,
        mul,
        div,
        grate,
        less,
        equal,
        not,
        mod,
        memory,
        and,
        or,
        xor,
        dup2,
        dup3,
        //chyvstva:
        rand,
        btime,
        time,
        bot,
        rbot,
        food,
        nrj,
        posx,
        posy,
        gen,
        fgen,
        mut,
        //deistvia...
        wait,
        photosyntes,
        rep,
        sex,
        Rrot,
        Lrot,
        walk,
        atack,
        suicide,
        //constants...
        c0,
        c1,
        c2,
        c5,
        c11,
    }

    static class Visualize
    {
        static string GtypeToSrt(Gtype type)
        {
            string caption;
            switch (type)
            {
                case Gtype.gen:
                    caption = "gn";
                    break;
                case Gtype.fgen:
                    caption = "fgn";
                    break;
                case Gtype.mut:
                    caption = "mut";
                    break;
                case Gtype.posx:
                    caption = "x";
                    break;
                case Gtype.posy:
                    caption = "y";
                    break;
                case Gtype.time:
                    caption = "t";
                    break;
                case Gtype.start:
                    caption = "st";
                    break;
                case Gtype.stop:
                    caption = "sp";
                    break;
                case Gtype.input:
                    caption = "in";
                    break;
                case Gtype.output:
                    caption = "out";
                    break;
                case Gtype.skip:
                    caption = "sk";
                    break;
                case Gtype.undo:
                    caption = "un";
                    break;
                case Gtype.add:
                    caption = "+";
                    break;
                case Gtype.sub:
                    caption = "-";
                    break;
                case Gtype.mul:
                    caption = "*";
                    break;
                case Gtype.div:
                    caption = "/";
                    break;
                case Gtype.grate:
                    caption = ">";
                    break;
                case Gtype.less:
                    caption = "<";
                    break;
                case Gtype.equal:
                    caption = "=";
                    break;
                case Gtype.not:
                    caption = "not";
                    break;
                case Gtype.mod:
                    caption = "mod";
                    break;
                case Gtype.memory:
                    caption = "mem";
                    break;
                case Gtype.and:
                    caption = "and";
                    break;
                case Gtype.or:
                    caption = "or";
                    break;
                case Gtype.xor:
                    caption = "xor";
                    break;
                case Gtype.dup2:
                    caption = "";
                    break;
                case Gtype.dup3:
                    caption = "";
                    break;
                case Gtype.rand:
                    caption = "rnd";
                    break;
                case Gtype.btime:
                    caption = "bt";
                    break;
                case Gtype.bot:
                    caption = "bt";
                    break;
                case Gtype.rbot:
                    caption = "rbt";
                    break;
                case Gtype.food:
                    caption = "fd";
                    break;
                case Gtype.nrj:
                    caption = "en";
                    break;
                case Gtype.wait:
                    caption = "w";
                    break;
                case Gtype.photosyntes:
                    caption = "ph";
                    break;
                case Gtype.rep:
                    caption = "rp";
                    break;
                case Gtype.sex:
                    caption = "sex";
                    break;
                case Gtype.Rrot:
                    caption = "rtrn";
                    break;
                case Gtype.Lrot:
                    caption = "ltrn";
                    break;
                case Gtype.walk:
                    caption = "wk";
                    break;
                case Gtype.atack:
                    caption = "atk";
                    break;
                case Gtype.suicide:
                    caption = "su";
                    break;
                case Gtype.c0:
                    caption = "0";
                    break;
                case Gtype.c1:
                    caption = "1";
                    break;
                case Gtype.c2:
                    caption = "2";
                    break;
                case Gtype.c5:
                    caption = "5";
                    break;
                case Gtype.c11:
                    caption = "11";
                    break;
                default:
                    caption = "z";
                    break;
            }
            return caption;
        }
        static void DisplayText(int cx, int cy, int size, int interval, string display)
        {
            void Interp(string cmds, int pos)
            {
                int s = pos * interval * size;

                int[] cords = Array.ConvertAll(cmds.ToCharArray(), x => int.Parse(x.ToString()) * size);
                for (int i = 2; i < cords.Length; i += 2)
                {
                    SDL_RenderDrawLine(vren, cords[i - 2] + s + cx, cords[i - 1] + cy, cords[i] + s + cx, cords[i + 1] + cy);
                }
            }

            string text = display;
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '0':
                        Interp("0004242000", i);
                        break;
                    case '1':
                        Interp("141001", i);
                        break;
                    case '2':
                        Interp("011021220424", i);
                        break;
                    case '3':
                        Interp("001021120212231404", i);
                        break;
                    case '4':
                        Interp("0002222420", i);
                        break;
                    case '5':
                        Interp("20000112222404", i);
                        break;
                    case '6':
                        Interp("200004242202", i);
                        break;
                    case '7':
                        Interp("0020211214", i);
                        break;
                    case '8':
                        Interp("002021030424230100", i);
                        break;
                    case '9':
                        Interp("042420000222", i);
                        break;
                    case 'a':
                        Interp("04011021242202", i);
                        break;
                    case 'b':
                        Interp("00102112021223140400", i);
                        break;
                    case 'c':
                        Interp("20000424", i);
                        break;
                    case 'd':
                        Interp("00041423211000", i);
                        break;
                    case 'e':
                        Interp("20000222020424", i);
                        break;
                    case 'f':
                        Interp("040222020020", i);
                        break;
                    case 'g':
                        Interp("200004242212", i);
                        break;
                    case 'h':
                        Interp("000402222024", i);
                        break;
                    case 'i':
                        Interp("002010140424", i);
                        break;
                    case 'j':
                        Interp("002010140403", i);
                        break;
                    case 'k':
                        Interp("00040212201224", i);
                        break;
                    case 'l':
                        Interp("000424", i);
                        break;
                    case 'm':
                        Interp("0400122024", i);
                        break;
                    case 'n':
                        Interp("04002420", i);
                        break;
                    case 'o':
                        Interp("0004242000", i);
                        break;
                    case 'p':
                        Interp("040010211202", i);
                        break;
                    case 'q':
                        Interp("242010011222", i);
                        break;
                    case 'r':
                        Interp("040010211202122324", i);
                        break;
                    case 's':
                        Interp("211001231403", i);
                        break;
                    case 't':
                        Interp("14100020", i);
                        break;
                    case 'u':
                        Interp("00042420", i);
                        break;
                    case 'v':
                        Interp("001420", i);
                        break;
                    case 'w':
                        Interp("0004122420", i);
                        break;
                    case 'x':
                        Interp("0024122004", i);
                        break;
                    case 'y':
                        Interp("04201200", i);
                        break;
                    case 'z':
                        Interp("00200424", i);
                        break;
                    case '*':
                        Interp("1113120222122301122103", i);
                        break;
                    case '+':
                        Interp("1113120222", i);
                        break;
                    case '-':
                        Interp("0222", i);
                        break;
                    case '=':
                        Interp("01210323", i);
                        break;
                    case '/':
                        Interp("2103", i);
                        break;
                    case '>':
                        Interp("012203", i);
                        break;
                    case '<':
                        Interp("210223", i);
                        break;
                    default:
                        break;
                }
            }
            return;
        }

        static public IntPtr vren;
        private static readonly List<(Gate, Node)> queue = new List<(Gate, Node)>();
        static readonly List<Edge> edges = new List<Edge>();

        class Edge
        {
            public int x1, y1, x2, y2, layer;
            public Edge(int x1, int y1, int x2, int y2, int layer)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
                this.layer = layer;
            }
        }
        class Node
        {
            public Gate g;
            public int x, y, w, h;
            public (int, int)[] input, output;
            public int layer;
            public Node(Gate gate, int x, int y, int layer)
            {
                g = gate;
                bool dupe = g.type == Gtype.dup2 || g.type == Gtype.dup3; //особая отрисовка (линии)
                this.x = x;
                this.y = dupe ? y + 5 : y;
                this.layer = layer;

                w = 40;
                h = dupe ? 0 : Math.Max(g.output.Length, g.input.Length) * 15 + 10;

                input = new (int, int)[g.input.Length];
                output = new (int, int)[g.output.Length];

                // Вычисляем начальные координаты для входных точек
                int inputSpacing = h / (g.input.Length + 1);
                for (int i = 0; i < g.input.Length; i++)
                {
                    input[i].Item1 = x + w; // Горизонтальная координата
                    input[i].Item2 = dupe ? y + 5 : y + (i + 1) * inputSpacing; // Вертикальная координата
                }

                // Вычисляем начальные координаты для выходных точек
                int outputSpacing = h / (g.output.Length + 1);
                for (int i = 0; i < g.output.Length; i++)
                {
                    output[i].Item1 = x; // Горизонтальная координата
                    output[i].Item2 = dupe ? y + 5 : y + (i + 1) * outputSpacing; // Вертикальная координата
                }
            }
        }

        public static void Brain(Bot e)
        {
            int layer = 0;
            queue.Clear();
            edges.Clear();
            int x = 10, y = 0;
            bool mem = true; //запомнить гейт
            (Gate, Node) flg = (null, null); //запомненый гейт

            foreach (Gate gate in e.gates) //определяем точки выхода
            {
                if (main.exp.Contains(gate.type) && gate.input[0].A != null)
                {
                    var node = new Node(gate, x, y, layer);
                    y += node.h + 5;
                    queue.Add((gate, node));
                    if (mem)
                    {
                        flg = queue.Last();
                        mem = false;
                    }
                }
            }

            for (int i = 0; i < queue.Count; i++)
            {
                var gate1 = queue[i].Item1;
                var node = queue[i].Item2;

                if (flg == queue[i]) //начинается новый слой
                {
                    //сдвигаем все ноды по центру
                    SDL_GetWindowSize(Program.vwin, out _, out int wy);
                    int dy = (wy - y) / 2;
                    foreach (var gt in queue)
                    {
                        if (gt.Item2.layer != layer)
                            continue;
                        Node nod = gt.Item2;
                        nod.y += dy;
                        for (int ii = 0; ii < nod.input.Length; ii++)
                            nod.input[ii].Item2 += dy;
                        for (int ii = 0; ii < nod.output.Length; ii++)
                            nod.output[ii].Item2 += dy;
                    }
                    foreach (var ed in edges)
                    {
                        if (ed.layer != layer)
                            continue;
                        ed.y2 += dy;
                    }

                    layer++;
                    mem = true;
                    x += 80;
                    y = 0;

                }

                for (int j = 0; j < gate1.input.Length; j++)
                {
                    foreach (var gate2 in e.gates)
                    {
                        for (int l = 0; l < gate2.output.Length; l++)
                        {
                            var l1 = gate2.output[l];
                            if (l1 == gate1.input[j])
                            {
                                /*
                                Посторайтесь понять)
                                Для каждого гейта проходим по всем его ВХОДНЫМ портам
                                И среди всех ВЫХОДНЫХ портов всех гейтов ищем пару

                                Тут мы её как раз нашли и теперь создаём новую ноду и линию
                                */

                                var node2 = new Node(gate2, x, y, layer);
                                y += node2.h == 0 ? 15 : node2.h + 5;
                                int x1 = node.input[j].Item1;
                                int y1 = node.input[j].Item2;
                                int x2 = node2.output[l].Item1;
                                int y2 = node2.output[l].Item2;
                                edges.Add(new Edge(x1, y1, x2, y2, layer));
                                queue.Add((gate2, node2));
                                if (mem)
                                {
                                    /*
                                     * это первая нода на новом слое и мы её запоминаем
                                     * когда мы начнём проходить по её ВХОДАМ это будет означать,
                                     * что начался новый слой
                                    */
                                    flg = queue.Last();
                                    mem = false;
                                }
                            }
                        }
                    }
                }
            }

            //отрисовываем ноды и линии
            SDL_SetRenderDrawColor(vren, 200, 200, 200, 255);
            SDL_RenderClear(vren);
            SDL_SetRenderDrawColor(vren, 0, 0, 0, 255);
            foreach (var en in queue)
            {
                Node node = en.Item2;
                string caption = GtypeToSrt(node.g.type);

                var rect = new SDL_Rect
                {
                    x = node.x,
                    y = node.y,
                    h = node.h,
                    w = node.w,
                };
                SDL_RenderDrawRect(vren, ref rect);

                //сокращения названий гейтов
                int tw = 2 * caption.Length + 4 * 2 * (caption.Length - 1);
                DisplayText(rect.x + (rect.w - tw) / 2, rect.y + (rect.h - 10) / 2, 2, 4, caption);
            }
            foreach (var edge in edges)
            {
                SDL_RenderDrawLine(vren, edge.x1, edge.y1, edge.x2, edge.y2);
            }
        }

        public static void Dna(Bot e)
        {
            SDL_SetRenderDrawColor(vren, 0, 0, 0, 255);
            SDL_GetWindowSize(Program.vwin, out int w, out int h);
            int cw = w / e.DNA.Length;

            for (int i = 0; i < Math.Max(e.FDNA.Length, e.DNA.Length); i++)
            {
                
            }
        }
    }
    internal class Program
    {
        //функции для генерации не серого цвета
        static void GenToColor(int seed, out byte red, out byte green, out byte blue)
        {
            bool IsGrayShade(int r, int g, int b)
            {
                // Определяем пороговое значение для различия между каналами
                int threshold = 20; // Порог можно настраивать

                // Проверяем, находятся ли значения каналов в пределах порогового значения друг от друга
                return Math.Abs(r - g) <= threshold &&
                       Math.Abs(r - b) <= threshold &&
                       Math.Abs(g - b) <= threshold;
            }

            Random random = new Random(seed);
            do
            {
                red = (byte)random.Next(256);
                green = (byte)random.Next(256);
                blue = (byte)random.Next(256);
            } while (IsGrayShade(red, green, blue));

            return;
        }

        static void RandomFill()
        {
            main.step = 0;
            main.cmap = new Bot[main.width, main.height];
            main.fmap = new Food[main.width, main.height];
            main.queue.Clear();

            for (int x = 0; x < main.width; x++)
            {
                for (int y = 0; y < main.height; y++)
                {
                    if (main.rnd.Next(0, 100) < 50)
                    {
                        main.cmap[x, y] = new Bot(x, y, 10);
                        main.queue.Add(main.cmap[x, y]);
                    }
                    else if (main.rnd.Next(0, 100) < 50)
                        main.fmap[x, y] = new Food(x, y, 10);
                }
            }

            /*
            gtype[] bestDNA = new gtype[] { gtype.start, gtype.dup2, gtype.output, gtype.atack, gtype.not, gtype.walk, gtype.input, gtype.or, gtype.bot, gtype.food, gtype.stop, gtype.start, gtype.and, gtype.output, gtype.rep, gtype.input, gtype.not, gtype.or, gtype.bot, gtype.food, gtype.equal, gtype.c0, gtype.mod, gtype.c2, gtype.time, gtype.stop, gtype.start, gtype.Lrot, gtype.input, gtype.rbot, gtype.stop, gtype.start, gtype.photosyntes, gtype.input, gtype.less, gtype.c5, gtype.nrj, gtype.stop, gtype.start, gtype.suicide, gtype.input, gtype.grate, gtype.mul, gtype.c11, gtype.mul, gtype.c11, gtype.c2, gtype.time, gtype.stop };
            main.cmap[30, 30] = new bot(30, 30, 10, 0, bestDNA);
            main.queue.Add(main.cmap[30, 30]);

            /*
            main.cmap[70, 50] = new bot(70, 50, 10, 666, new gtype[] { gtype.start, gtype.nrj, gtype.rep, gtype.dup3, gtype.input, gtype.and, gtype.posx, gtype.dup2, gtype.xor, gtype.input, gtype.Rrot, gtype.bot, gtype.walk, gtype.less, gtype.output, gtype.skip, gtype.Lrot, gtype.or, gtype.bot, gtype.rbot, gtype.bot, gtype.photosyntes, gtype.food, gtype.c2, gtype.start, gtype.grate, gtype.food, gtype.mod, gtype.div, gtype.div, gtype.walk, gtype.atack, gtype.undo, gtype.grate, gtype.atack, gtype.photosyntes, gtype.atack, gtype.photosyntes, gtype.grate, gtype.wait, gtype.rbot, gtype.undo, gtype.or, gtype.output, gtype.c1, gtype.rep, gtype.undo, gtype.rand, gtype.btime, gtype.atack, gtype.c2, gtype.rep, gtype.div, gtype.walk, gtype.rbot, gtype.equal, gtype.btime, gtype.Lrot, gtype.dup3, gtype.walk, gtype.Rrot, gtype.mod, gtype.grate, gtype.div, gtype.c11, gtype.c2, gtype.less, gtype.rbot, gtype.not, gtype.memory, gtype.bot, gtype.output, gtype.add, gtype.atack, gtype.dup2, gtype.xor, gtype.not, gtype.time, gtype.stop, gtype.grate, gtype.undo, gtype.undo, gtype.walk, gtype.memory, gtype.equal, gtype.or, gtype.food, gtype.equal, gtype.stop, gtype.time, gtype.wait, gtype.stop, gtype.grate, gtype.xor, gtype.c2, gtype.less, gtype.suicide, gtype.sub, gtype.output, gtype.c11, gtype.and, gtype.time, gtype.div, gtype.sub, gtype.equal, gtype.atack, gtype.food, gtype.and, gtype.time, gtype.Rrot, gtype.c5, gtype.dup3, gtype.Rrot, gtype.div, gtype.posy, gtype.grate, gtype.btime, gtype.c0, gtype.c2, gtype.rbot, gtype.not, gtype.output, gtype.time, gtype.Rrot, gtype.nrj, gtype.btime, gtype.walk, gtype.photosyntes, gtype.c1, gtype.start, gtype.time, gtype.undo, gtype.dup2, gtype.stop, gtype.suicide, gtype.rbot, gtype.c5, gtype.nrj, gtype.input, gtype.dup2, gtype.Rrot, gtype.grate, gtype.photosyntes, gtype.less, gtype.bot, gtype.div, gtype.less, gtype.stop, gtype.photosyntes, gtype.less, gtype.rep, gtype.rbot, gtype.add, gtype.rand, gtype.posy, gtype.equal, gtype.c11, gtype.and, gtype.btime, gtype.rep });
            main.queue.Add(main.cmap[70, 50]);
            */
        }

        public static IntPtr vwin = SDL_CreateWindow("321", main.winW + 30, 30, main.winW, main.winH, SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
        static void Main()
        {
            var win = SDL_CreateWindow("123", 30, 30, main.winW, main.winH, SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
            var ren = SDL_CreateRenderer(win, 0, SDL_RendererFlags.SDL_RENDERER_SOFTWARE);

            Visualize.vren = SDL_CreateRenderer(vwin, 1, SDL_RendererFlags.SDL_RENDERER_SOFTWARE);

            SDL_SetRenderDrawColor(Visualize.vren, 200, 200, 200, 255);
            SDL_RenderClear(Visualize.vren);
            SDL_RenderPresent(Visualize.vren);

            bool running = true;
            bool playing = true;

            RandomFill();

            while (running)
            {
                while (SDL_PollEvent(out SDL_Event e) == 1)
                {
                    switch (e.type)
                    {
                        case SDL_EventType.SDL_QUIT:
                            running = false;
                            break;
                        case SDL_EventType.SDL_MOUSEBUTTONUP:

                            int x, y;
                            SDL_GetMouseState(out x, out y);
                            x /= main.cw;
                            y /= main.ch;

                            if (e.window.windowID == 2)
                            {
                                Bot b = main.cmap[x, y];

                                switch (e.button.button)
                                {
                                    case (byte)SDL_BUTTON_LEFT:
                                        if (b != null)
                                        {
                                            Visualize.Brain(b);
                                            Visualize.Dna(b);
                                            SDL_RenderPresent(Visualize.vren);
                                        }
                                        break;
                                    case (byte)SDL_BUTTON_RIGHT:
                                        if (b == null)
                                            break;
                                        StringBuilder dna = new StringBuilder();
                                        bool first = true;
                                        foreach (var n in b.DNA)
                                        {
                                            if (!first)
                                                dna.Append(" ");
                                            dna.Append(n.ToString());
                                            first = false;
                                        }
                                        SDL_SetClipboardText(dna.ToString());
                                        break;
                                    case (byte)SDL_BUTTON_MIDDLE:
                                        if (b != null || main.fmap[x, y] != null)
                                            break;

                                        try
                                        {
                                            string[] tdna = SDL_GetClipboardText().Split();
                                            Gtype[] Idna = new Gtype[tdna.Length];
                                            for (int i = 0; i < Idna.Length; i++)
                                            {
                                                Idna[i] = (Gtype)Enum.Parse(typeof(Gtype), tdna[i]);
                                            }
                                            main.queue.Add(new Bot(x, y, 10, main.rnd.Next(int.MinValue, int.MaxValue), Idna));
                                            main.cmap[x, y] = main.queue.Last();
                                        }
                                        catch { };
                                        break;
                                }

                            }
                            break; //отображение мозга (сделать отображение днк)
                        case SDL_EventType.SDL_KEYUP:
                            int wx, wy;
                            SDL_GetWindowSize(vwin, out wx, out wy);
                            switch (e.key.keysym.sym)
                            {
                                case SDL_Keycode.SDLK_g:
                                    RandomFill();
                                    break;
                                case SDL_Keycode.SDLK_SPACE:
                                    playing = !playing;
                                    break;
                                case SDL_Keycode.SDLK_ESCAPE:
                                    running = false;
                                    break;
                                case SDL_Keycode.SDLK_LEFT:
                                    SDL_SetWindowSize(vwin, wx - 10, wy);
                                    break;
                                case SDL_Keycode.SDLK_UP:
                                    SDL_SetWindowSize(vwin, wx, wy - 10);
                                    break;
                                case SDL_Keycode.SDLK_RIGHT: //->
                                    SDL_SetWindowSize(vwin, wx + 10, wy);
                                    break;
                                case SDL_Keycode.SDLK_DOWN: // \/
                                    SDL_SetWindowSize(vwin, wx, wy + 10);
                                    break;
                                case SDL_Keycode.SDLK_RCTRL:
                                    SDL_SetWindowSize(vwin, 500, 500);
                                    break;
                            }
                            break;

                    }
                }

                if (playing)
                    main.Tick();

                SDL_SetWindowTitle(win, main.step.ToString());
                SDL_SetRenderDrawColor(ren, 0, 0, 0, 255);
                SDL_RenderClear(ren);
                foreach (Bot bot in main.queue)
                {
                    var rect = new SDL_Rect { x = bot.x * main.cw, y = bot.y * main.ch, w = main.cw, h = main.ch };

                    GenToColor(bot.gen, out byte r, out byte g, out byte b);

                    SDL_SetRenderDrawColor(ren, r, g, b, 255);
                    SDL_RenderFillRect(ren, ref rect);
                }
                SDL_SetRenderDrawColor(ren, 50, 50, 50, 255);
                foreach (Food f in main.fmap)
                {
                    if (f == null)
                        continue;
                    var rect = new SDL_Rect { x = f.x * main.cw, y = f.y * main.ch, w = main.cw, h = main.ch };
                    SDL_RenderFillRect(ren, ref rect);
                }
                SDL_RenderPresent(ren);
            }
        }

    }

    class Gate
    {
        public Link[] input;
        public Link[] output;
        public Gtype type;
        public Gate(Gtype type)
        {
            this.type = type;
            switch (type)
            {
                case Gtype.and:
                case Gtype.or:
                case Gtype.xor:
                case Gtype.mod:
                case Gtype.mul:
                case Gtype.grate:
                case Gtype.less:
                case Gtype.equal:
                case Gtype.div:
                case Gtype.sub:
                case Gtype.add:
                    input = new Link[2];
                    output = new Link[1];
                    break;

                case Gtype.memory:
                case Gtype.not:
                    input = new Link[1];
                    output = new Link[1];
                    break;

                case Gtype.mut:
                case Gtype.fgen:
                case Gtype.gen:
                case Gtype.btime:
                case Gtype.c0:
                case Gtype.c1:
                case Gtype.c2:
                case Gtype.c5:
                case Gtype.c11:
                case Gtype.posy:
                case Gtype.posx:
                case Gtype.nrj:
                case Gtype.food:
                case Gtype.rbot:
                case Gtype.bot:
                case Gtype.time:
                case Gtype.rand:
                    input = new Link[0];
                    output = new Link[1];
                    break;
                case Gtype.suicide:
                case Gtype.photosyntes:
                case Gtype.atack:
                case Gtype.rep:
                case Gtype.sex:
                case Gtype.Rrot:
                case Gtype.Lrot:
                case Gtype.walk:
                case Gtype.wait:
                    input = new Link[1];
                    output = new Link[0];
                    break;

                case Gtype.dup2:
                    input = new Link[1];
                    output = new Link[2];
                    break;
                case Gtype.dup3:
                    input = new Link[1];
                    output = new Link[3];
                    break;
            }
        }

        public float Compute()
        {
            switch (type)
            {
                case Gtype.add:
                    output[0].f = input[0].f + input[1].f;
                    break;
                case Gtype.sub:
                    output[0].f = input[0].f - input[1].f;
                    break;
                case Gtype.mul:
                    output[0].f = input[0].f * input[1].f;
                    break;
                case Gtype.div:
                    output[0].f = input[0].f / input[1].f;
                    break;
                case Gtype.equal:
                    output[0].f = input[0].f == input[1].f ? 1 : 0;
                    break;
                case Gtype.mod:
                    output[0].f = input[0].f % input[1].f;
                    break;
                case Gtype.grate:
                    output[0].f = input[0].f > input[1].f ? 1 : 0;
                    break;
                case Gtype.less:
                    output[0].f = input[0].f < input[1].f ? 1 : 0;
                    break;
                case Gtype.not:
                    output[0].f = 1 - input[0].f;
                    break;
                case Gtype.memory:
                    output[0].f += input[0].f;
                    break;
                case Gtype.and:
                    output[0].f = input[0].f > 0.5 && input[1].f > 0.5 ? 1 : 0;
                    break;
                case Gtype.or:
                    output[0].f = input[0].f > 0.5 || input[1].f > 0.5 ? 1 : 0;
                    break;
                case Gtype.xor:
                    output[0].f = input[0].f > 0.5 ^ input[1].f > 0.5 ? 1 : 0;
                    break;
                case Gtype.dup2:
                    output[0].f = input[0].f;
                    output[1].f = input[0].f;
                    break;
                case Gtype.dup3:
                    output[0].f = input[0].f;
                    output[1].f = input[0].f;
                    output[2].f = input[0].f;
                    break;

                case Gtype.c0:
                    output[0].f = 0;
                    break;
                case Gtype.c1:
                    output[0].f = 1;
                    break;
                case Gtype.c2:
                    output[0].f = 2;
                    break;
                case Gtype.c5:
                    output[0].f = 5;
                    break;
                case Gtype.c11:
                    output[0].f = 11;
                    break;

            }

            if (input.Length == 0)
                return -1;

            return input[0].f;
        }
    }

    class Link
    {
        public Gate A;
        public Gate B;
        public float f;

        public Link(Gate A, Gate B)
        {
            this.A = A;
            this.B = B;
        }
    }

    static class main
    {
        public static readonly int winW = 600, winH = 600;
        public static int width = 300, height = 300, cw = winW / width, ch = winH / height;
        public static Bot[,] cmap = new Bot[width, height];
        public static Food[,] fmap = new Food[width, height];

        public static List<Bot> queue = new List<Bot>();
        public static List<Bot> bqueue = new List<Bot>();

        public static Random rnd = new Random();
        public static int step;
        public static Gtype[] exp = { Gtype.wait, Gtype.photosyntes, Gtype.rep, Gtype.sex, Gtype.Rrot, Gtype.Lrot, Gtype.walk, Gtype.atack, Gtype.suicide };

        public static void Tick()
        {
            foreach (Bot bot in queue)
            {
                bot.Init();
            }
            queue = new List<Bot>(bqueue);
            bqueue.Clear();

            step++;
        }

        static public Gtype Think(Bot e, out float signal)
        {
            List<Gate> queue = new List<Gate>();

            foreach (Gate gate in e.gates)
            {
                if (exp.Contains(gate.type))
                    queue.Add(gate);
            } //определяем точки выхода (действия)
            for (int i = 0; i < queue.Count; i++)
            {
                var gate = queue[i];
                foreach (var link in gate.input)
                    if (!queue.Contains(link.A) && link.A != null)
                        queue.Add(link.A);
            } //продолжаем очередь
            for (int i = queue.Count - 1; i >= 0; i--) //сворачиваем очередь
            {
                var gate = queue[i];
                signal = gate.Compute();
                if (signal > 0 && exp.Contains(gate.type)) //выполнение действия
                    return gate.type;
            }

            signal = 0;
            return Gtype.wait;
        } //вызывает гейты в нужном порядке
    }

    class Food
    {
        public int x, y;
        public float nrj;
        public Food(int x, int y, float nrj)
        {
            this.x = x;
            this.y = y;
            this.nrj = nrj;
        }
    }

    class Bot
    {
        public Bot(int x, int y, float nrj)
        {
            this.x = x;
            this.y = y;
            this.nrj = nrj;
            mut = 0;
            DNA = new Gtype[main.rnd.Next(10, 200)];
            gen = fgen = main.rnd.Next(int.MinValue, int.MaxValue);
            btime = main.step;

            var nykls = Enum.GetValues(typeof(Gtype));
            for (int i = 0; i < DNA.Length; i++)
            {
                DNA[i] = (Gtype)nykls.GetValue(main.rnd.Next(1, nykls.Length));
            }
            FDNA = DNA;
            Translation();
        }

        public Bot(int x, int y, float nrj, int gen, Gtype[] DNA)
        {
            this.x = x;
            this.y = y;
            this.nrj = nrj;
            this.gen = fgen = gen;
            mut = 0;
            btime = main.step;
            this.DNA = FDNA = DNA;

            Translation();
        }

        public static List<T[]> SplitByElement<T>(T[] array, T delimiter)
        {
            List<T[]> result = new List<T[]>();
            List<T> temp = new List<T>();

            foreach (var item in array)
            {
                if (item.Equals(delimiter))
                {
                    result.Add(temp.ToArray());
                    temp.Clear();
                }
                else
                {
                    temp.Add(item);
                }
            }

            // Добавляем остаток после последнего разделителя, если он есть
            if (temp.Count > 0)
            {
                result.Add(temp.ToArray());
            }

            return result;
        }

        public static T[] CombineWithDelimiter<T>(T[][] arrays, T delimiter)
        {
            List<T> result = new List<T>();

            for (int i = 0; i < arrays.Length; i++)
            {
                if (i > 0)
                {
                    result.Add(delimiter);
                }
                result.AddRange(arrays[i]);
            }

            return result.ToArray();
        }

        private void Mutation(Gtype[] fDNA)
        {
            bool m = main.rnd.Next(0, 100) < 5;
            bool SorE;
            if (m && main.rnd.Next(0, 100) < 6)
            {
                SorE = main.rnd.Next(0, 100) < 50;
                if (main.rnd.Next(0, 100) < 50)
                {
                    DNA = new Gtype[fDNA.Length + 1];
                    if (SorE)
                        Array.Copy(fDNA, 0, DNA, 1, fDNA.Length);
                    else
                        Array.Copy(fDNA, DNA, fDNA.Length);
                }
                else
                {
                    DNA = new Gtype[fDNA.Length - 1];
                    if (SorE)
                        Array.Copy(fDNA, 1, DNA, 0, DNA.Length);
                    else
                        Array.Copy(fDNA, DNA, DNA.Length);
                    mut++;
                }
            } //изменение длины днк
            else
            {
                DNA = new Gtype[fDNA.Length];
                Array.Copy(fDNA, DNA, fDNA.Length);
            }

            var nykls = Enum.GetValues(typeof(Gtype));
            for (int i = 0; i < DNA.Length; i++)
            {
                if (m && main.rnd.Next(0, 100) < 3 || DNA[i] == 0)
                {
                    DNA[i] = (Gtype)nykls.GetValue(main.rnd.Next(1, nykls.Length));
                    mut++;
                }
            } //мутации
        } //копирует днк к текущему боту со случайными изменениями

        private Gtype[] Crossingover(Gtype[] dna1, Gtype[] dna2)
        {
            Gtype[][] Sdna1 = SplitByElement(dna1, Gtype.start).ToArray();
            Gtype[][] Sdna2 = SplitByElement(dna2, Gtype.start).ToArray();
            Gtype[][] dna = new Gtype[Math.Max(Sdna1.Length, Sdna2.Length)][];

            for (int i = 0; i < dna.Length; i++)
            {
                if (i >= Sdna1.Length || (i < Sdna2.Length && main.rnd.Next(100) < 50))
                {
                    dna[i] = Sdna2[i];
                    if (i < Sdna1.Length)
                    {
                        for (int j = 0; j < Math.Min(Sdna1[i].Length, Sdna2[i].Length); j++)
                        {
                            if (i >= Sdna1.Length)
                                mut++;
                            else
                            {
                                if (Sdna1[i][j] != Sdna2[i][j])
                                    mut++;
                            }
                        }
                        mut += Math.Abs(Sdna1.Length - Sdna2.Length);
                    }
                    else
                        mut += Sdna2[i].Length;
                }
                else
                    dna[i] = Sdna1[i];
            }

            return CombineWithDelimiter(dna, Gtype.start);
        }

        public Bot(int x, int y, float nrj, Bot f)
        {
            this.x = x;
            this.y = y;
            this.nrj = nrj;
            FDNA = f.FDNA;
            mut = f.mut;
            btime = main.step;
            dx = f.dx;
            dy = f.dy;
            rot = f.rot;
            fgen = f.fgen;

            Mutation(f.DNA);

            if (mut > 2)
            {
                gen = main.rnd.Next(int.MinValue, int.MaxValue);
                mut = 0;
            } //критичное колличество мутаций
            else
                gen = f.gen;

            Translation();
        }

        public Bot(int x, int y, float nrj, Bot p1, Bot p2)
        {
            //условно p1 - отец - инициатор, p2 - мать - создаёт бота
            this.x = x;
            this.y = y;
            this.nrj = nrj;
            FDNA = p1.FDNA;
            mut = p1.mut + p2.mut;
            btime = main.step;
            dx = p2.dx;
            dy = p2.dy;
            rot = p2.rot;
            fgen = p1.fgen;

            Mutation(Crossingover(p1.DNA, p2.DNA));

            if (mut > 2)
            {
                gen = main.rnd.Next(int.MinValue, int.MaxValue);
                mut = 0;
            } //критичное колличество мутаций
            else
                gen = p1.gen;

            Translation();
        }

        public int x, y; //сколько пропускать нуклеотидов (для визуализации)
        private int dx = 1;
        private int dy = 1; //направление поворота
        private int mut; //сколько мутаций было
        private int rot; //поворот
        private readonly int btime;
        public int gen, fgen; //ген и ген отца
        float nrj;
        public List<Gate> gates = new List<Gate>();
        public Gtype[] DNA, FDNA;
        private static readonly Gtype[] coddons = { Gtype.start, Gtype.input, Gtype.output, Gtype.stop, Gtype.skip, Gtype.undo, Gtype.empty }; //специальный кодоны

        public void Translation()
        {
            List<Link> ins = new List<Link>();
            List<Link> outs = new List<Link>();
            Gtype wt = Gtype.start;
            int adr = -1; //адресc подключения

            for (int i = 0; i < DNA.Length; i++)
            {
                if (coddons.Contains(DNA[i]))
                {
                    switch (DNA[i])
                    {
                        case Gtype.stop:
                            ins.Clear();
                            outs.Clear();
                            break;
                        case Gtype.skip:
                            //adr--;
                            break;
                        case Gtype.undo:
                            //adr++;
                            break;
                        default:
                            wt = DNA[i];
                            break;
                    }
                    continue;
                } //специальные кодоны

                Gate gate = new Gate(DNA[i]);

                void VFill()
                {
                    for (int j = 0; j < gate.input.Length; j++)
                    {
                        if (gate.input[j] != null)
                            continue;
                        gate.input[j] = new Link(null, gate);
                        ins.Add(gate.input[j]);
                    }
                    for (int j = 0; j < gate.output.Length; j++)
                    {
                        if (gate.output[j] != null)
                            continue;
                        gate.output[j] = new Link(gate, null);
                        outs.Add(gate.output[j]);
                    }
                }
                Link link;

                switch (wt)
                {
                    case Gtype.input: //подключение ко входу
                        if (gate.output.Length == 0 || ins.Count == 0)
                            continue;
                        int fiadr = adr - ins.Count * (int)Math.Floor(adr / (decimal)ins.Count);

                        link = ins[fiadr];
                        link.A = gate;
                        gate.output[0] = link;

                        ins.Remove(link);
                        break;
                    case Gtype.output: //подключение к выходу
                        if (gate.input.Length == 0 || outs.Count == 0)
                            continue;
                        int foadr = adr - outs.Count * (int)Math.Floor(adr / (decimal)outs.Count);

                        link = outs[foadr];
                        link.B = gate;
                        gate.input[0] = link;

                        outs.Remove(link);
                        break;
                }

                VFill(); //заполняем порты
                gates.Add(gate);
            }
        } //создание мозга

        public void Init()
        {
            int tx = (x + dx + main.width) % main.width;
            int ty = (y + dy + main.height) % main.height;

            nrj--;
            if (nrj <= 0)
            {
                main.cmap[x, y] = null;
                main.fmap[x, y] = new Food(x, y, 10);
                return;
            } //смерть

            foreach (var gate in gates)
            {
                switch (gate.type)
                {
                    case Gtype.mut:
                        gate.output[0].f = mut;
                        break;
                    case Gtype.fgen:
                        gate.output[0].f = fgen;
                        break;
                    case Gtype.gen:
                        gate.output[0].f = gen;
                        break;
                    case Gtype.time:
                        gate.output[0].f = main.step;
                        break;
                    case Gtype.rand:
                        gate.output[0].f = main.rnd.Next(0, 10);
                        break;
                    case Gtype.bot:
                        gate.output[0].f = main.cmap[tx, ty] != null ? 1 : 0;
                        break;
                    case Gtype.rbot:
                        bool rb = false;
                        if (main.cmap[tx, ty] != null)
                            rb = gen == main.cmap[tx, ty].gen;
                        gate.output[0].f = rb ? 1 : 0;
                        break;
                    case Gtype.food:
                        gate.output[0].f = main.fmap[tx, ty] == null ? 0 : 1;
                        break;
                    case Gtype.nrj:
                        gate.output[0].f = nrj;
                        break;
                    case Gtype.posx:
                        gate.output[0].f = x;
                        break;
                    case Gtype.posy:
                        gate.output[0].f = y;
                        break;
                    case Gtype.btime:
                        gate.output[0].f = btime;
                        break;
                }
            } //обновление сенсоров

            switch (main.Think(this, out float signal))
            {
                case Gtype.photosyntes: //фотосинтез
                    nrj += 2;
                    break;
                case Gtype.rep: //размножение
                    /*
                    if (main.cmap[tx, ty] == null && main.fmap[tx, ty] == null)
                    {
                        main.cmap[tx, ty] = new Bot(tx, ty, 5, this);
                        main.bqueue.Add(main.cmap[tx, ty]);
                    }
                    break;
                    */
                case Gtype.sex: //половое размножение
                    if (main.cmap[tx, ty] != null) //есть ли второй родитель
                    {
                        Bot p2 = main.cmap[tx, ty];
                        int tx2 = (p2.x + p2.dx + main.width) % main.width;
                        int ty2 = (p2.y + p2.dy + main.height) % main.height;

                        if (main.cmap[tx2, ty2] == null && main.fmap[tx2, ty2] == null) //пусто ли перед вторым родителем
                        {
                            main.cmap[tx2, ty2] = new Bot(tx2, ty2, 5, this, p2);
                            main.bqueue.Add(main.cmap[tx2, ty2]);
                        }
                    }
                    break;
                case Gtype.Rrot: //поворот 1
                    rot = (rot + 1) % 8;
                    break;
                case Gtype.Lrot: //поворот 2
                    rot = (rot + 7) % 8;
                    break;
                case Gtype.walk: //ходьба
                    if (main.cmap[tx, ty] == null && main.fmap[tx, ty] == null)
                    {
                        main.cmap[tx, ty] = (Bot)main.cmap[x, y].MemberwiseClone();
                        main.cmap[x, y] = null;
                        x = tx;
                        y = ty;
                    }
                    break;
                case Gtype.atack: //атака
                    if (main.cmap[tx, ty] != null)
                    {
                        float dnrj = main.cmap[tx, ty].nrj * 0.5F;
                        main.cmap[tx, ty].nrj -= dnrj;
                        nrj += dnrj;
                    }
                    if (main.fmap[tx, ty] != null)
                    {
                        nrj += main.fmap[tx, ty].nrj;
                        main.fmap[tx, ty] = null;
                    }
                    break;
                case Gtype.suicide: //суицид
                    main.fmap[x, y] = new Food(x, y, nrj);
                    main.cmap[x, y] = null;
                    return;
            }

            switch (rot)
            {
                case 0:
                    dx = 1;
                    dy = 0;
                    break;
                case 1:
                    dx = 1;
                    dy = 1;
                    break;
                case 2:
                    dx = 0;
                    dy = 1;
                    break;
                case 3:
                    dx = -1;
                    dy = 1;
                    break;
                case 4:
                    dx = -1;
                    dy = 0;
                    break;
                case 5:
                    dx = -1;
                    dy = -1;
                    break;
                case 6:
                    dx = 0;
                    dy = -1;
                    break;
                case 7:
                    dx = 1;
                    dy = -1;
                    break;
            } //rotating

            main.bqueue.Add(this);
        }

    }
}
