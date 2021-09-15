using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms2048
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CreateField();
        }
        private Random rnd = new Random();

        private enum Direction { Up, Down, Left, Right };

        // Стандартные настройки => field: 4/4 case: 75/75
        private int fieldWidth = 4, fieldHeight = 4;
        public int caseWidth = 75, caseHeight = 75;

        public int Score = 0;
        public bool isEnd = false;
        public Case[,] mapCases;
        public Case[,] oldMapCases;
        public Label[,] mapPics;
        public Vector[,] spawnPoints;

        // Создаём игровое поле (начало игры)
        private void CreateField()
        {
            mapCases = new Case[fieldWidth, fieldHeight];
            oldMapCases = new Case[fieldWidth, fieldHeight];
            mapPics = new Label[fieldWidth, fieldHeight];
            spawnPoints = new Vector[fieldWidth, fieldHeight];

            InitSpawnPoints();

            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    Label Case = new Label();
                    Case.BackColor = Color.FromArgb(255, 77, 77, 77);
                    Case.Size = new Size(caseWidth, caseHeight);
                    Case.Location = new Point(spawnPoints[i, j].X, spawnPoints[i, j].Y);
                    this.Controls.Add(Case);
                }
            }

            SpawnCase(0, 0);
            SpawnCase(1, 0);
            SpawnCase(3, 2);
        }

        private void InitSpawnPoints()
        {
            for (int i = 0; i < fieldWidth; i++)
            {
                for (int j = 0; j < fieldHeight; j++)
                {
                    spawnPoints[i, j] = new Vector(10 + (caseWidth + 5) * j, 90 + (caseHeight + 5) * i);
                }
            }
        }

        // Спавним ячейку с цифрой
        private void SpawnCase(int x, int y, int value = 2)
        {
            if (mapCases[x, y] != null)
                return;

            Label Case = new Label();
            Case.BackColor = Color.FromArgb(44, 93, 247);
            Case.Font = new Font(FontFamily.GenericSansSerif, 16);
            Case.TextAlign = ContentAlignment.MiddleCenter;
            Case.Location = new Point(spawnPoints[x, y].X, spawnPoints[x, y].Y);
            Case.Size = new Size(caseWidth, caseHeight);
            Case.Text = value.ToString();
            mapCases[x, y] = new Case(x, y, value);
            mapPics[x, y] = Case;
            this.Controls.Add(mapPics[x, y]);
            mapPics[x, y].BringToFront();
        }

        private void NextTurn()
        {
            bool isSpawned = false;
            int x, y;
            while(!isSpawned)
            {
                if (!isEmptyCases())
                    break;

                x = rnd.Next(0, fieldWidth);
                y = rnd.Next(0, fieldHeight);

                if(mapCases[x, y] == null)
                {
                    int value = 2;
                    if (rnd.Next(1, 5) == 4)
                        value = 4;
                    SpawnCase(x, y, value);
                    isSpawned = true;
                }
            }

            CheckForEnd();
        }

        private void SaveMap()
        {
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    oldMapCases[i, j] = mapCases[i, j];
                }
            }
        }

        private bool isCorMaps()
        {
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    if (oldMapCases[i, j] != mapCases[i, j])
                        return false;
                }
            }

            return true;
        }

        private bool is2048Case()
        {
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    if (mapCases[i, j] != null && mapCases[i, j].Value == 2048)
                        return true;
                }
            }

            return false;
        }

        private void CheckForEnd()
        {
            if (is2048Case())
            {
                // Победа
                MessageBox.Show("Вы выйграли! Заработано очков: " + Score, "Победа!");
                isEnd = true;
            }
            else if (isCorMaps() && !isEmptyCases())
            {
                // Проигрыш
                MessageBox.Show("Вы проиграли :c", "Поражение...");
                isEnd = true;
            }
        }

        private bool isEmptyCases()
        {
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    if (mapCases[i, j] == null)
                        return true;
                }
            }
            return false;
        }

        private void MoveCases(Direction dir)
        {
            SaveMap();

            switch (dir)
            {
                case Direction.Up:
                    for (int i = 1; i < fieldHeight; i++)
                    {
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            if (mapPics[i, j] != null)
                            {
                                for (int m = i - 1; m >= 0; m--)
                                {
                                    if(mapPics[m, j] == null)
                                    {
                                        mapPics[m + 1, j].Location = new Point(spawnPoints[m, j].X, spawnPoints[m, j].Y);
                                        mapPics[m, j] = mapPics[m + 1, j];
                                        mapPics[m + 1, j] = null;
                                        mapCases[m, j] = mapCases[m + 1, j];
                                        mapCases[m + 1, j] = null;
                                    }
                                    else if(mapPics[m, j] != null && mapCases[m, j] != null && mapCases[m, j].Value == mapCases[m + 1, j].Value)
                                    {
                                        mapCases[m, j].Value *= 2;
                                        Score += mapCases[m, j].Value;
                                        mapCases[m + 1, j] = null;
                                        this.Controls.Remove(mapPics[m + 1, j]);
                                        mapPics[m + 1, j] = null;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Direction.Down:
                    for (int i = fieldHeight - 1; i >= 0; i--)
                    {
                        for (int j = 0; j < fieldWidth; j++)
                        {
                            if (mapCases[i, j] != null)
                            {
                                for (int m = i + 1; m < fieldHeight; m++)
                                {
                                    if (mapPics[m, j] == null)
                                    {
                                        mapPics[m - 1, j].Location = new Point(spawnPoints[m, j].X, spawnPoints[m, j].Y);
                                        mapPics[m, j] = mapPics[m - 1, j];
                                        mapPics[m - 1, j] = null;
                                        mapCases[m, j] = mapCases[m - 1, j];
                                        mapCases[m - 1, j] = null;
                                    }
                                    else if (mapPics[m, j] != null && mapCases[m, j] != null && mapCases[m, j].Value == mapCases[m - 1, j].Value)
                                    {
                                        mapCases[m, j].Value *= 2;
                                        Score += mapCases[m, j].Value;
                                        mapCases[m - 1, j] = null;
                                        this.Controls.Remove(mapPics[m - 1, j]);
                                        mapPics[m - 1, j] = null;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Direction.Left:
                    for (int i = 0; i < fieldHeight; i++)
                    {
                        for (int j = 1; j < fieldWidth; j++)
                        {
                            if (mapCases[i, j] != null)
                            {
                                for (int m = j - 1; m >= 0; m--)
                                {
                                    if (mapPics[i, m] == null)
                                    {
                                        mapPics[i, m + 1].Location = new Point(spawnPoints[i, m].X, spawnPoints[i, m].Y);
                                        mapPics[i, m] = mapPics[i, m + 1];
                                        mapPics[i, m + 1] = null;
                                        mapCases[i, m] = mapCases[i, m + 1];
                                        mapCases[i, m + 1] = null;
                                    }
                                    else if (mapPics[i, m] != null && mapCases[i, m] != null && mapCases[i, m].Value == mapCases[i, m + 1].Value)
                                    {
                                        mapCases[i, m].Value *= 2;
                                        Score += mapCases[i, m].Value;
                                        mapCases[i, m + 1] = null;
                                        this.Controls.Remove(mapPics[i, m + 1]);
                                        mapPics[i, m + 1] = null;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Direction.Right:
                    for (int i = 0; i < fieldHeight; i++)
                    {
                        for (int j = fieldWidth - 1; j >= 0; j--)
                        {
                            if (mapCases[i, j] != null)
                            {
                                for (int m = j + 1; m < fieldWidth; m++)
                                {
                                    if (mapPics[i, m] == null)
                                    {
                                        mapPics[i, m - 1].Location = new Point(spawnPoints[i, m].X, spawnPoints[i, m].Y);
                                        mapPics[i, m] = mapPics[i, m - 1];
                                        mapPics[i, m - 1] = null;
                                        mapCases[i, m] = mapCases[i, m - 1];
                                        mapCases[i, m - 1] = null;
                                    }
                                    else if (mapPics[i, m] != null && mapCases[i, m] != null && mapCases[i, m].Value == mapCases[i, m - 1].Value)
                                    {
                                        mapCases[i, m].Value *= 2;
                                        Score += mapCases[i, m].Value;
                                        mapCases[i, m - 1] = null;
                                        this.Controls.Remove(mapPics[i, m - 1]);
                                        mapPics[i, m - 1] = null;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            ReDrawCases();
            NextTurn();

            //string st = "";
            //for (int i = 0; i < fieldWidth; i++)
            //{
            //    for (int j = 0; j < fieldHeight; j++)
            //    {
            //        if (mapCases[i, j] != null)
            //            st += mapCases[i, j].ToString() + "\n";
            //    }
            //}

            //MessageBox.Show(st);
        }

        private void ReDrawCases()
        {
            int val = 0;
            for (int i = 0; i < fieldHeight; i++)
            {
                for (int j = 0; j < fieldWidth; j++)
                {
                    if (mapCases[i, j] != null)
                    {
                        val = mapCases[i, j].Value;
                        mapPics[i, j].Text = val.ToString();
                        if (val == 4)
                            mapPics[i, j].BackColor = Color.FromArgb(38, 65, 201);
                        if (val == 8)
                            mapPics[i, j].BackColor = Color.FromArgb(26, 28, 163);
                        if (val == 16)
                            mapPics[i, j].BackColor = Color.FromArgb(79, 26, 163);
                        if (val == 32)
                            mapPics[i, j].BackColor = Color.FromArgb(110, 48, 186);
                        if (val == 64)
                            mapPics[i, j].BackColor = Color.FromArgb(135, 75, 212);
                        if (val == 128)
                            mapPics[i, j].BackColor = Color.FromArgb(155, 91, 234);
                        if (val == 256)
                            mapPics[i, j].BackColor = Color.FromArgb(236, 89, 255);
                        if (val == 512)
                            mapPics[i, j].BackColor = Color.FromArgb(255, 24, 164);
                        if (val == 1024)
                            mapPics[i, j].BackColor = Color.FromArgb(255, 0, 78);
                        if (val == 2048)
                            mapPics[i, j].BackColor = Color.FromArgb(255, 0, 0);
                    }
                }
            }

            scoreLabel.Text = "Очки: " + Score;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (isEnd)
                return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveCases(Direction.Up);
                    break;
                case Keys.Down:
                    MoveCases(Direction.Down);
                    break;
                case Keys.Left:
                    MoveCases(Direction.Left);
                    break;
                case Keys.Right:
                    MoveCases(Direction.Right);
                    break;
            }
        }
    }
}
