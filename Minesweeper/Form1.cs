using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Configuration;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private Image imBombRed = Properties.Resources.bomb_red;
        private Image imBomb = Properties.Resources.bomb;
        private Image imFlag = Properties.Resources.flag;
        private Image imQuestionMark = Properties.Resources.questionmark;
        private Image im0 = Properties.Resources.gray;
        private Image im1 = Properties.Resources._1;
        private Image im2 = Properties.Resources._2;
        private Image im3 = Properties.Resources._3;
        private Image im4 = Properties.Resources._4;
        private Image im5 = Properties.Resources._5;
        private Image im6 = Properties.Resources._6;
        private Image im7 = Properties.Resources._7;
        private Image im8 = Properties.Resources._8;
        private System.Media.SoundPlayer bombSound = new System.Media.SoundPlayer(Properties.Resources.bombSound);
        private System.Media.SoundPlayer fanfarySound = new System.Media.SoundPlayer(Properties.Resources.fanfareSound);

        private const int size = 16;
        private int flagsLeft;
        private int fieldsToReveal;
        private Field[,] fields = new Field[size + 2, size + 2];
        private Button[,] buttons = new Button[size, size];
        private int mines;
        private bool isGameOver = false;
        private bool isGameOn = false;
        private Stopwatch stopWatch = new Stopwatch();
        TimeSpan timeSpan = new TimeSpan();

        public Form1()
        {
            InitializeComponent();
        }
        private void startButton_Click(object sender, EventArgs e)
        {
            if (IsLevelChosen() == true)
            {
                isGameOver = false;
                isGameOn = true;
                CreateFields();
                CreateButtons();
                ClearButtonsText();
                WhatIsChecked();
                DrawMines();
                CalculateSolution();
                SetString();
                stopWatch.Restart();
            }
            else
            {
                MessageBox.Show("Choose level first!", "Choose level!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void CreateFields()
        {
            for (int i = 0; i < size + 2; i++)
            {
                for (int j = 0; j < size + 2; j++)
                {
                    if (i == 0 || i == size + 1 || j == 0 || j == size + 1) fields[i, j] = new Field(true);
                    else fields[i, j] = new Field(false);
                }
            }
        }
        private void CreateButtons()
        {
            int i = size - 1;
            int j = size - 1;
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    if (c.Name.Contains("button"))
                    {
                        buttons[i,j] = (Button)c;
                        j--;
                        if (j < 0)
                        {
                            i--;
                            j += 16; ;
                        }
                    }
                }
            }
        }
        private void ClearButtonsText()
        {
            foreach (Button c in buttons)
            {
                c.Text = "";
                c.Image = null;
                c.BackColor = Color.Transparent;
            }
        }
        private void WhatIsChecked()
        {
            if (Beginner.Checked)
            {
                mines = 25;
            }
            if (Advanced.Checked)
            {
                mines = 35;
            }
            if (Expert.Checked)
            {
                mines = 45;
            }
            flagsLeft = mines;
            fieldsToReveal = size * size - mines;
            MinesLeft.Text = "Flags left: " + flagsLeft.ToString();
        }
        private void DrawMines()
        {
            List<int> availableFields = new List<int>();
            for(int i=0; i<size*size; i++)
            {
                availableFields.Add(i);
            }
            Random rand = new Random();
            int whereBombPlaced, drawedNumber, x, y;
            for(int i=0; i<mines;i++)
            {
                drawedNumber = rand.Next(0, size*size - i);
                whereBombPlaced = availableFields[drawedNumber];
                availableFields.RemoveAt(drawedNumber);
                x = whereBombPlaced / 16;
                y = whereBombPlaced % 16;
                fields[x+1, y+1].IsMine = true;
            }
        }

        private void CalculateSolution()
        {
            for(int i=1; i<size+1; i++)
            {
                for(int j=1; j<size+1; j++)
                {
                    for(int k = i-1; k<=i+1; k++)
                    {
                        for(int l = j-1; l<=j+1; l++)
                        {
                            if (!(k == i && l == j) && fields[k,l].IsMine == true) fields[i, j].HowManyBombsAround++;
                        }
                    }
                }
            }
        }
        private void SetString()
        {
            for (int i = 0; i < fields.GetLength(0); i++)
            {
                for (int j = 0; j < fields.GetLength(1); j++)
                {
                    if (fields[i, j].IsMine == true) fields[i, j].BombsString = "X";
                    else fields[i, j].BombsString = fields[i, j].HowManyBombsAround.ToString();
                }
            }
        }
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (isGameOver == false && isGameOn == true)
            {
                int x = WhichButtonClicked((Button)sender).Item1;
                int y = WhichButtonClicked((Button)sender).Item2;
                if (fields[x + 1, y + 1].wasClicked == false)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (fields[x + 1, y + 1].rightClick != "F")
                        {
                            fields[x + 1, y + 1].wasClicked = true;
                            fieldsToReveal--;
                            if (fieldsToReveal == 0 && flagsLeft == 0)
                            {
                                YouWin();
                            }
                            if (fields[x + 1, y + 1].IsMine == true)
                            {
                                isGameOver = true;
                                ((Button)sender).Image = imBombRed;
                                ((Button)sender).BackColor = Color.Red;
                                bombSound.Play();
                                MessageBox.Show("Unfortunately, you lost! :(", "Game over!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                DisplaySolution();
                            }
                            else
                            {
                                switch (fields[x + 1, y + 1].HowManyBombsAround)
                                {
                                    case 0:
                                        ((Button)sender).Image = im0;
                                        ShowNeighbours(sender,e);
                                        break;
                                    case 1:
                                        ((Button)sender).Image = im1;
                                        break;
                                    case 2:
                                        ((Button)sender).Image = im2;
                                        break;
                                    case 3:
                                        ((Button)sender).Image = im3;
                                        break;
                                    case 4:
                                        ((Button)sender).Image = im4;
                                        break;
                                    case 5:
                                        ((Button)sender).Image = im5;
                                        break;
                                    case 6:
                                        ((Button)sender).Image = im6;
                                        break;
                                    case 7:
                                        ((Button)sender).Image = im7;
                                        break;
                                    case 8:
                                        ((Button)sender).Image = im8;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        switch (fields[x + 1, y + 1].rightClick)
                        {
                            case "":
                                if (flagsLeft > 0)
                                {
                                    fields[x + 1, y + 1].rightClick = "F";
                                    flagsLeft--;
                                    MinesLeft.Text = "Flags left: " + flagsLeft.ToString();
                                    ((Button)sender).Image = imFlag;
                                    if (fieldsToReveal == 0 && flagsLeft == 0)
                                    {
                                        YouWin();
                                    }
                                }
                                break;
                            case "F":
                                fields[x + 1, y + 1].rightClick = "?";
                                flagsLeft++;
                                MinesLeft.Text = "Flags left: " + flagsLeft.ToString();
                                ((Button)sender).Image = imQuestionMark;
                                break;
                            case "?":
                                fields[x + 1, y + 1].rightClick = "";
                                ((Button)sender).Image = null;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        private void ShowNeighbours(object sender,MouseEventArgs e)
        {
            int i = 0;
            int j = 0;
            foreach (Button c in buttons)
            {
                if (((Button)sender).Name == buttons[i, j].Name)
                {
                    break;
                }
                else
                {
                    j++;
                    if (j > 15)
                    {
                        i++;
                        j -= 16;
                    }
                }
            }
            for(int x = i - 1; x < i + 2; x++)
            {
                for(int y = j - 1; y < j + 2; y++)
                {
                    if (x >= 0 && x <= 15 && y >= 0 && y <= 15)
                    {
                        if(buttons[x, y].Image == null)
                        Button_MouseUp(buttons[x, y], e);
                    }
                }
            }
        }
        private void DisplaySolution()
        {
            foreach (Button c in buttons)
            {
                int x = WhichButtonClicked(c).Item1;
                int y = WhichButtonClicked(c).Item2;
                switch(fields[x + 1, y + 1].BombsString)
                {
                    case "0":
                        c.Image = im0;
                        break;
                    case "1":
                        c.Image = im1;
                        break;
                    case "2":
                        c.Image = im2;
                        break;
                    case "3":
                        c.Image = im3;
                        break;
                    case "4":
                        c.Image = im4;
                        break;
                    case "5":
                        c.Image = im5;
                        break;
                    case "6":
                        c.Image = im6;
                        break;
                    case "7":
                        c.Image = im7;
                        break;
                    case "8":
                        c.Image = im8;
                        break;
                    case "X":
                        c.Image = imBomb;
                        break;
                    default:
                        break;
                }   
            }
        }
        private void YouWin()
        {
            isGameOver = true;
            stopWatch.Stop();
            timeSpan = stopWatch.Elapsed;
            fanfarySound.Play();
            MessageBox.Show("Congratulations, you won!!! :)\nElapsed time: " + timeSpan.ToString("mm\\:ss\\.ff"), "You won!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private Tuple<int,int> WhichButtonClicked(Button c)
        {
            int whichButtonClicked = int.Parse(c.Name.Remove(0, 6)) - 1;
            int x, y;
            x = whichButtonClicked / 16;
            y = whichButtonClicked % 16;
            return Tuple.Create(x,y);
        }
        private void CheckedChange(object sender, EventArgs e)
        {
            startButton.Text = "Start (" + ((RadioButton)sender).Name + ")";
        }
        private bool IsLevelChosen()
        {
            if (Beginner.Checked == false && Advanced.Checked == false && Expert.Checked == false) return false;
            else return true;
        }
        private void howToPlay_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Win by putting flags on bomb fields and revealing all the non-bomb fields! \n" +
                "- Reveal fields using mouse left-click \n" +
                "- Put flags using mouse right-click","How to play",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
