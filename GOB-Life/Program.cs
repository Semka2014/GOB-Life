using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SDL2;
using static SDL2.SDL;

namespace GOB_Life
{
    enum gtype
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

    static class visualize
    {
        static string GtypeToSrt(gtype type)
        {
            string caption;
            switch (type)
            {
                case gtype.gen:
                    caption = "gn";
                    break;
                case gtype.fgen:
                    caption = "fgn";
                    break;
                case gtype.mut:
                    caption = "mut";
                    break;
                case gtype.posx:
                    caption = "x";
                    break;
                case gtype.posy:
                    caption = "y";
                    break;
                case gtype.time:
                    caption = "t";
                    break;
                case gtype.start:
                    caption = "st";
                    break;
                case gtype.stop:
                    caption = "sp";
                    break;
                case gtype.input:
                    caption = "in";
                    break;
                case gtype.output:
                    caption = "out";
                    break;
                case gtype.skip:
                    caption = "sk";
                    break;
                case gtype.undo:
                    caption = "un";
                    break;
                case gtype.add:
                    caption = "+";
                    break;
                case gtype.sub:
                    caption = "-";
                    break;
                case gtype.mul:
                    caption = "*";
                    break;
                case gtype.div:
                    caption = "/";
                    break;
                case gtype.grate:
                    caption = ">";
                    break;
                case gtype.less:
                    caption = "<";
                    break;
                case gtype.equal:
                    caption = "=";
                    break;
                case gtype.not:
                    caption = "not";
                    break;
                case gtype.mod:
                    caption = "mod";
                    break;
                case gtype.memory:
                    caption = "mem";
                    break;
                case gtype.and:
                    caption = "and";
                    break;
                case gtype.or:
                    caption = "or";
                    break;
                case gtype.xor:
                    caption = "xor";
                    break;
                case gtype.dup2:
                    caption = "";
                    break;
                case gtype.dup3:
                    caption = "";
                    break;
                case gtype.rand:
                    caption = "rnd";
                    break;
                case gtype.btime:
                    caption = "bt";
                    break;
                case gtype.bot:
                    caption = "bt";
                    break;
                case gtype.rbot:
                    caption = "rbt";
                    break;
                case gtype.food:
                    caption = "fd";
                    break;
                case gtype.nrj:
                    caption = "en";
                    break;
                case gtype.wait:
                    caption = "w";
                    break;
                case gtype.photosyntes:
                    caption = "ph";
                    break;
                case gtype.rep:
                    caption = "rp";
                    break;
                case gtype.Rrot:
                    caption = "rtrn";
                    break;
                case gtype.Lrot:
                    caption = "ltrn";
                    break;
                case gtype.walk:
                    caption = "wk";
                    break;
                case gtype.atack:
                    caption = "atk";
                    break;
                case gtype.suicide:
                    caption = "su";
                    break;
                case gtype.c0:
                    caption = "0";
                    break;
                case gtype.c1:
                    caption = "1";
                    break;
                case gtype.c2:
                    caption = "2";
                    break;
                case gtype.c5:
                    caption = "5";
                    break;
                case gtype.c11:
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
                    SDL.SDL_RenderDrawLine(vren, cords[i - 2] + s + cx, cords[i - 1] + cy, cords[i] + s + cx, cords[i + 1] + cy);
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
        static List<(gate, node)> queue = new List<(gate, node)>();
        static List<edge> edges = new List<edge>();

        class edge
        {
            public int x1, y1, x2, y2, layer;
            public edge(int x1, int y1, int x2, int y2, int layer)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
                this.layer = layer;
            }
        }
        class node
        {
            public gate g;
            public int x, y, w, h;
            public (int, int)[] input, output;
            public int layer;
            public node(gate gate, int x, int y, int layer)
            {
                g = gate;
                bool dupe = g.type == gtype.dup2 || g.type == gtype.dup3; //особая отрисовка (линии)
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

        public static void Brain(bot e)
        {
            int layer = 0;
            queue.Clear();
            edges.Clear();
            int x = 10, y = 0;
            bool mem = true; //запомнить гейт
            (gate, node) flg = (null, null); //запомненый гейт

            foreach (gate gate in e.gates) //определяем точки выхода
            {
                if (main.exp.Contains(gate.type) && gate.input[0].A != null)
                {
                    var node = new node(gate, x, y, layer);
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
                    SDL_GetWindowSize(Program.vwin, out int wx, out int wy);
                    int dy = (wy - y) / 2;
                    foreach (var gt in queue)
                    {
                        if (gt.Item2.layer != layer)
                            continue;
                        node nod = gt.Item2;
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

                                var node2 = new node(gate2, x, y, layer);
                                y += node2.h == 0 ? 15 : node2.h + 5;
                                int x1 = node.input[j].Item1;
                                int y1 = node.input[j].Item2;
                                int x2 = node2.output[l].Item1;
                                int y2 = node2.output[l].Item2;
                                edges.Add(new edge(x1, y1, x2, y2, layer));
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
            foreach(var en in queue)
            {
                node node = en.Item2;
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

        public static void Dna(bot e)
        {
            SDL_SetRenderDrawColor(vren, 0, 0, 0, 255);
            SDL_GetWindowSize(Program.vwin, out int w, out int h);
            int cw = w / e.DNA.Length;

            for (int i = 0; i < Math.Max(e.FDNA.Length, e.DNA.Length); i++)
            {
                if (i - e.startsk < e.DNA.Length && i < e.FDNA.Length && i >= e.startsk && i - e.startsk >= 0)
                {
                    if (e.FDNA[i] == e.DNA[i - e.startsk])
                        SDL_SetRenderDrawColor(vren, 0, 255, 0, 255);
                    else
                        SDL_SetRenderDrawColor(vren, 255, 0, 0, 255);
                }
                else
                {
                    SDL_SetRenderDrawColor(vren, 0, 0, 255, 255);
                }

                var rect = new SDL_Rect {
                    x = cw * i + 5,
                    y = h - 15,
                    w = cw - 1,
                    h = 10,
                };
                SDL_RenderFillRect(vren, ref rect);
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
            main.cmap = new bot[main.width, main.height];
            main.fmap = new food[main.width, main.height];
            main.queue.Clear();

            for (int x = 0; x < main.width; x++)
            {
                for (int y = 0; y < main.height; y++)
                {
                    if (main.rnd.Next(0, 100) < 20)
                    {
                        main.cmap[x, y] = new bot(x, y, 10);
                        main.queue.Add(main.cmap[x, y]);
                    }
                    else if (main.rnd.Next(0, 100) < 50)
                        main.fmap[x, y] = new food(x, y, 10);
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

        public static IntPtr vwin = SDL_CreateWindow("321", 600, 100, 500, 500, SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
        static void Main(string[] args)
        {
            var win = SDL_CreateWindow("123", 100, 100, 500, 500, SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
            var ren = SDL_CreateRenderer(win, 0, SDL_RendererFlags.SDL_RENDERER_SOFTWARE);

            visualize.vren = SDL_CreateRenderer(vwin, 1, SDL_RendererFlags.SDL_RENDERER_SOFTWARE);

            SDL_SetRenderDrawColor(visualize.vren, 200, 200, 200, 255);
            SDL_RenderClear(visualize.vren);
            SDL_RenderPresent(visualize.vren);

            bool running = true;
            bool playing = true;

            RandomFill();

            while (running)
            {
                while(SDL_PollEvent(out SDL_Event e) == 1)
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
                                bot b = main.cmap[x, y];

                                switch (e.button.button)
                                {
                                    case (byte)SDL_BUTTON_LEFT:
                                        if (b != null)
                                        {
                                            visualize.Brain(b);
                                            visualize.Dna(b);
                                            SDL_RenderPresent(visualize.vren);
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
                                            gtype[] Idna = new gtype[tdna.Length];
                                            for (int i = 0; i < Idna.Length; i++)
                                            {
                                                Idna[i] = (gtype)Enum.Parse(typeof(gtype), tdna[i]);
                                            }
                                            main.queue.Add(new bot(x, y, 10, main.rnd.Next(int.MinValue, int.MaxValue), Idna));
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
                    main.tick();

                SDL_SetWindowTitle(win, main.step.ToString());
                SDL_SetRenderDrawColor(ren, 0, 0, 0, 255);
                SDL_RenderClear(ren);
                foreach (bot bot in main.queue)
                {
                    var rect = new SDL_Rect { x = bot.x * main.cw, y = bot.y * main.ch, w = main.cw, h = main.ch };
                    byte r, g, b;
                    GenToColor(bot.gen, out r, out g, out b);

                    SDL_SetRenderDrawColor(ren, r, g, b, 255);
                    SDL_RenderFillRect(ren, ref rect);
                }
                SDL_SetRenderDrawColor(ren, 50, 50, 50, 255);
                foreach (food f in main.fmap)
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

    class gate
    {
        public link[] input;
        public link[] output;
        public gtype type;
        public gate(gtype type)
        {
            this.type = type;
            switch(type)
            {
                case gtype.and:
                case gtype.or:
                case gtype.xor:
                case gtype.mod:
                case gtype.mul:
                case gtype.grate:
                case gtype.less:
                case gtype.equal:
                case gtype.div:
                case gtype.sub:
                case gtype.add:
                    input = new link[2];
                    output = new link[1];
                break;

                case gtype.memory:
                case gtype.not:
                    input = new link[1];
                    output = new link[1];
                break;

                case gtype.mut:
                case gtype.fgen:
                case gtype.gen:
                case gtype.btime:
                case gtype.c0:
                case gtype.c1:
                case gtype.c2:
                case gtype.c5:
                case gtype.c11:
                case gtype.posy:
                case gtype.posx:
                case gtype.nrj:
                case gtype.food:
                case gtype.rbot:
                case gtype.bot:
                case gtype.time:
                case gtype.rand:
                    input = new link[0];
                    output = new link[1];
                break;
                case gtype.suicide:
                case gtype.photosyntes:
                case gtype.atack:
                case gtype.rep:
                case gtype.Rrot:
                case gtype.Lrot:
                case gtype.walk:
                case gtype.wait:
                    input = new link[1];
                    output = new link[0];
                    break;

                case gtype.dup2:
                    input = new link[1];
                    output = new link[2];
                    break;
                case gtype.dup3:
                    input = new link[1];
                    output = new link[3];
                    break;
            }
        }

        public float count()
        {
            switch (type)
            {
                case gtype.add:
                    output[0].f = input[0].f + input[1].f;
                    break;
                case gtype.sub:
                    output[0].f = input[0].f - input[1].f;
                    break;
                case gtype.mul:
                    output[0].f = input[0].f * input[1].f;
                    break;
                case gtype.div:
                    output[0].f = input[0].f / input[1].f;
                    break;
                case gtype.equal:
                    output[0].f = input[0].f == input[1].f ? 1 : 0;
                    break;
                case gtype.mod:
                    output[0].f = input[0].f % input[1].f;
                    break;
                case gtype.grate:
                    output[0].f = input[0].f > input[1].f ? 1 : 0;
                    break;
                case gtype.less:
                    output[0].f = input[0].f < input[1].f ? 1 : 0;
                    break;
                case gtype.not:
                    output[0].f = 1 - input[0].f;
                    break;
                case gtype.memory:
                    output[0].f += input[0].f;
                    break;
                case gtype.and:
                    output[0].f = input[0].f > 0.5 && input[1].f > 0.5 ? 1 : 0;
                    break;
                case gtype.or:
                    output[0].f = input[0].f > 0.5 || input[1].f > 0.5 ? 1 : 0;
                    break;
                case gtype.xor:
                    output[0].f = input[0].f > 0.5 ^ input[1].f > 0.5 ? 1 : 0;
                    break;
                case gtype.dup2:
                    output[0].f = input[0].f;
                    output[1].f = input[0].f;
                    break;
                case gtype.dup3:
                    output[0].f = input[0].f;
                    output[1].f = input[0].f;
                    output[2].f = input[0].f;
                    break;

                case gtype.c0:
                    output[0].f = 0;
                    break;
                case gtype.c1:
                    output[0].f = 1;
                    break;
                case gtype.c2:
                    output[0].f = 2;
                    break;
                case gtype.c5:
                    output[0].f = 5;
                    break;
                case gtype.c11:
                    output[0].f = 11;
                    break;

            }

            if (input.Length == 0)
                return -1;

            return input[0].f;
        }
    }
    
    class link
    {
        public gate A;
        public gate B;
        public float f;

        public link(gate A, gate B)
        {
            this.A = A;
            this.B = B;
        }
    }

    static class main
    {
        public static int width = 250, height = 250, cw = 500 / width, ch = 500 / height;
        public static bot[,] cmap = new bot[width, height];
        public static food[,] fmap = new food[width, height];

        public static List<bot> queue = new List<bot>();
        public static List<bot> bqueue = new List<bot>();

        public static Random rnd = new Random();
        public static int step;
        public static gtype[] exp = { gtype.wait, gtype.photosyntes, gtype.rep, gtype.Rrot, gtype.Lrot, gtype.walk, gtype.atack, gtype.suicide };

        public static void tick()
        {
            foreach (bot bot in queue)
            {
                bot.Init();
            }
            queue = new List<bot>(bqueue);
            bqueue.Clear();

            step++;
        }

        static public gtype think(bot e, out float signal)
        {
            List<gate> queue = new List<gate>();

            foreach (gate gate in e.gates)
            { 
                if (exp.Contains(gate.type))
                    queue.Add(gate);
            } //определяем точки выхода (действия)
            for (int i = 0; i < queue.Count; i++)
            {
                var gate = queue[i];
                foreach(var link in gate.input)
                    if (!queue.Contains(link.A) && link.A != null)
                        queue.Add(link.A);
            } //продолжаем очередь
            for (int i = queue.Count - 1; i >= 0; i--) //сворачиваем очередь
            {
                var gate = queue[i];
                signal = gate.count();
                if (signal > 0 && exp.Contains(gate.type)) //выполнение действия
                    return gate.type;
            }

            signal = 0;
            return gtype.wait;
        } //вызывает гейты в нужном порядке
    }

    class food
    {
        public int x, y;
        public float nrj;
        public food(int x, int y, float nrj)
        {
            this.x = x;
            this.y = y;
            this.nrj = nrj;
        }
    }

    class bot
    {
        public bot(int x, int y, float nrj)
        {
            this.x = x;
            this.y = y;
            this.nrj = nrj;
            mut = 0;
            DNA = new gtype[main.rnd.Next(10, 200)];
            gen = fgen = main.rnd.Next(int.MinValue, int.MaxValue);
            btime = main.step;

            var nykls = Enum.GetValues(typeof(gtype));
            for (int i = 0; i < DNA.Length; i++)
            {
                DNA[i] = (gtype)nykls.GetValue(main.rnd.Next(1, nykls.Length));
            }
            FDNA = DNA;
            Translation();
        }

        public bot(int x, int y, float nrj, int gen, gtype[] DNA)
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

        public bot(int x, int y, float nrj, bot f)
        {
            this.x = x;
            this.y = y;
            this.nrj = nrj;
            FDNA = f.FDNA;
            mut = f.mut;
            btime = main.step;
            startsk = f.startsk;
            dx = f.dx;
            dy = f.dy;
            rot = f.rot;
            fgen = f.fgen;

            bool m = main.rnd.Next(0, 100) < 5;
            bool SorE = false;
            if (m && main.rnd.Next(0, 100) < 6)
            {
                SorE = main.rnd.Next(0, 100) < 50;
                if (main.rnd.Next(0, 100) < 50)
                {
                    DNA = new gtype[f.DNA.Length + 1];
                    if (SorE)
                    {
                        Array.Copy(f.DNA, 0, DNA, 1, f.DNA.Length);
                        startsk--;
                    }
                    else
                        Array.Copy(f.DNA, DNA, f.DNA.Length);
                }
                else
                {
                    DNA = new gtype[f.DNA.Length - 1];
                    if (SorE)
                    {
                        Array.Copy(f.DNA, 1, DNA, 0, DNA.Length);
                        startsk++;
                    }
                    else
                        Array.Copy(f.DNA, DNA, DNA.Length);
                    mut++;
                }
            } //изменение длины днк
            else
            {
                DNA = new gtype[f.DNA.Length];
                Array.Copy(f.DNA, DNA, f.DNA.Length);
            }

            var nykls = Enum.GetValues(typeof(gtype));
            for (int i = 0; i < DNA.Length; i++)
            {
                if (m && main.rnd.Next(0, 100) < 3 || DNA[i] == 0)
                {
                    DNA[i] = (gtype)nykls.GetValue(main.rnd.Next(1, nykls.Length));
                    mut++;
                }
            } //мутации

            if (mut > 2)
            {
                gen = main.rnd.Next(int.MinValue, int.MaxValue);
                mut = 0;
            } //критичное колличество мутаций
            else
                gen = f.gen;

            Translation();
        }

        public int x, y, startsk;
        int dx = 1, dy = 1, mut, rot, btime;
        public int gen, fgen;
        float nrj;
        public List<gate> gates = new List<gate>();
        public gtype[] DNA, FDNA;

        static gtype[] coddons = { gtype.start, gtype.input, gtype.output, gtype.stop, gtype.skip, gtype.undo, gtype.empty }; //специальный кодоны

        public void Translation()
        {
            List<link> ins = new List<link>();
            List<link> outs = new List<link>();
            gtype wt = gtype.start;
            int adr = -1; //адресc подключения

            for (int i = 0; i < DNA.Length;i++)
            {
                if (coddons.Contains(DNA[i]))
                {
                    switch (DNA[i])
                    {
                        case gtype.stop:
                            ins.Clear();
                            outs.Clear();
                            break;
                        case gtype.skip:
                            //adr--;
                            break;
                        case gtype.undo:
                            //adr++;
                            break;
                        default:
                            wt = DNA[i];
                            break;
                    }
                    continue;
                } //специальные кодоны

                gate gate = new gate(DNA[i]);

                void VFill()
                {
                    for (int j = 0; j < gate.input.Length; j++)
                    {
                        if (gate.input[j] != null)
                            continue;
                        gate.input[j] = new link(null, gate);
                        ins.Add(gate.input[j]);
                    }
                    for (int j = 0; j < gate.output.Length; j++)
                    {
                        if (gate.output[j] != null)
                            continue;
                        gate.output[j] = new link(gate, null);
                        outs.Add(gate.output[j]);
                    }
                }
                link link;

                switch (wt)
                {
                    case gtype.input: //подключение ко входу
                        if (gate.output.Length == 0 || ins.Count == 0)
                            continue;
                        int fiadr = adr - ins.Count * (int)Math.Floor(adr / (decimal)ins.Count);

                        link = ins[fiadr];
                        link.A = gate;
                        gate.output[0] = link;

                        ins.Remove(link);
                        break;
                    case gtype.output: //подключение к выходу
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
                main.fmap[x, y] = new food(x, y, 10);
                return;
            } //смерть

            foreach (var gate in gates)
            {
                switch (gate.type)
                {
                    case gtype.mut:
                        gate.output[0].f = mut;
                        break;
                    case gtype.fgen:
                        gate.output[0].f = fgen;
                        break;
                    case gtype.gen:
                        gate.output[0].f = gen;
                        break;
                    case gtype.time:
                        gate.output[0].f = main.step;
                        break;
                    case gtype.rand:
                        gate.output[0].f = main.rnd.Next(0, 10);
                        break;
                    case gtype.bot:
                        gate.output[0].f = main.cmap[tx, ty] != null ? 1 : 0;
                        break;
                    case gtype.rbot:
                        bool rb = false;
                        if (main.cmap[tx, ty] != null)
                            rb = gen == main.cmap[tx, ty].gen;
                        gate.output[0].f = rb ? 1 : 0;
                        break;
                    case gtype.food:
                        gate.output[0].f = main.fmap[tx, ty] == null ? 0 : 1;
                        break;
                    case gtype.nrj:
                        gate.output[0].f = nrj;
                        break;
                    case gtype.posx:
                        gate.output[0].f = x;
                        break;
                    case gtype.posy:
                        gate.output[0].f = y;
                        break;
                    case gtype.btime:
                        gate.output[0].f = btime;
                        break;
                }
            } //обновление сенсоров

            switch (main.think(this, out float signal))
            {
                case gtype.photosyntes: //фотосинтез
                    nrj += 2;
                    break;
                case gtype.rep: //размножение
                    if (main.cmap[tx, ty] == null && main.fmap[tx, ty] == null)
                    {
                        main.cmap[tx, ty] = new bot(tx, ty, 5, this);
                        main.bqueue.Add(main.cmap[tx, ty]);
                    }
                    break;
                case gtype.Rrot: //поворот 1
                    rot = (rot + 1) % 8;
                    break;
                case gtype.Lrot: //поворот 2
                    rot = (rot + 7) % 8;
                    break;
                case gtype.walk: //ходьба
                    if (main.cmap[tx, ty] == null && main.fmap[tx, ty] == null)
                    {
                        main.cmap[tx, ty] = (bot)main.cmap[x, y].MemberwiseClone();
                        main.cmap[x, y] = null;
                        x = tx;
                        y = ty;
                    }
                    break;
                case gtype.atack: //атака
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
                case gtype.suicide: //суицид
                    main.fmap[x, y] = new food(x, y, nrj);
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
