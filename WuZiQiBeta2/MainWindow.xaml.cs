using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WuZiQiBeta2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public class Point
    {
        public int x;
        public int y;
        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
    public class Wuziqi
    {
        private static int[] dx = { 1, 0, -1, 1, -1, 0, 1, -1 };
        private static int[] dy = { 0, 1, -1, -1, 0, -1, 1, 1 };
        public int[,] ChessBoard; //表示棋盘0为空,1为黑,2为白,黑为人,白为PC
        private long[,] ScoreTable; //保存每个点的重要程度
        private long[,] TempTable; //保存如果PC下了这个点之后的重要程度
        public Wuziqi()
        {
            ChessBoard = new int[15, 15];
            ScoreTable = new long[15, 15];
            TempTable = new long[15, 15];
        }
        /// <summary>
        /// 判断此点是否在棋盘内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool JudgeBound(int x, int y)
        {
            if (x >= 0 && x < 15 && y >= 0 && y < 15) return true;
            return false;
        }
        /// <summary>
        /// 判断这一步之后是否分出胜负
        /// </summary>
        /// <param name="x">x坐标</param>
        /// <param name="y">y坐标</param>
        /// <param name="val">值的类型1为黑，2为白</param>
        /// <returns></returns>
        public bool judgeWin(int x, int y, int val)
        {
            int StretchLen;
            for (int i = 0; i < 4; i++)
            {
                StretchLen = 1;
                int tx = x + dx[i], ty = y + dy[i];
                while (JudgeBound(tx, ty) && ChessBoard[tx, ty] == val)
                {
                    tx += dx[i];
                    ty += dy[i];
                    StretchLen++;
                }
                tx = x + dx[i + 4];
                ty = y + dy[i + 4];
                while (JudgeBound(tx, ty) && ChessBoard[tx, ty] == val)
                {
                    tx += dx[i + 4];
                    ty += dy[i + 4];
                    StretchLen++;
                }
                if (StretchLen >= 5) return true;
            }
            return false;
        }
        /// <summary>
        /// 计算某个空点的分值
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        int CalValue(int x, int y, int val)
        {
            int Result = 0;
            int[] StretchLen = { 1, 1, 1, 1 };
            int[] EndPoint = { 0, 0, 0, 0 };
            int[] Situation = new int[7];//0为活五，1为活四，2为死四，3为活三，4为死三，5为活二，6为死二
            for (int i = 0; i < 8; i++)
            {
                int tx = x + dx[i];
                int ty = y + dy[i];
                while (true)
                {
                    if (JudgeBound(tx, ty))
                    {
                        if (ChessBoard[tx, ty] == val)
                        {
                            StretchLen[i % 4]++;
                            tx += dx[i];
                            ty += dy[i];
                        }
                        else if (ChessBoard[tx, ty] == 0)
                        {
                            break;
                        }
                        else
                        {
                            EndPoint[i % 4]++;
                            break;
                        }
                    }
                    else
                    {
                        EndPoint[i % 4]++;
                        break;
                    }

                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (StretchLen[i] >= 5) Situation[0]++;
                else if (StretchLen[i] == 4 && EndPoint[i] == 0) Situation[1]++;
                else if (StretchLen[i] == 4 && EndPoint[i] == 1) Situation[2]++;
                else if (StretchLen[i] == 3 && EndPoint[i] == 0) Situation[3]++;
                else if (StretchLen[i] == 3 && EndPoint[i] == 1) Situation[4]++;
                else if (StretchLen[i] == 2 && EndPoint[i] == 0) Situation[5]++;
                else if (StretchLen[i] == 2 && EndPoint[i] == 1) Situation[6]++;
            }
            if (Situation[0] > 0) Result += 100000;
            else if (Situation[1] > 0 || (Situation[2] >= 2) || (Situation[2] > 0 && Situation[3] > 0)) Result += 10000;
            else if (Situation[3] >= 2) Result += 5000;
            else if (Situation[3] > 0 && Situation[4] > 0) Result += 1000;
            else if (Situation[2] > 0) Result += 500;
            else if (Situation[3] > 0) Result += 300;
            else if (Situation[5] >= 2) Result += 100;
            else if (Situation[4] > 0) Result += 50;
            else if (Situation[5] > 0) Result += 10;
            else if (Situation[6] > 0) Result += 2;
            return Result;
        }
        /// <summary>
        /// 极大极小搜索出如果计算机下了（x，y）时，对于人下的所有点中，伤害最小的点。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public long FindMax()
        {
            long maxr = -1;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if(ChessBoard[i,j] == 0)
                    {
                        long temp = CalValue(i, j, 1);
                        if (temp > maxr) maxr = temp;
                    }
                }
            }
            return maxr;
        }

        public Point FindMin()
        {
            long minr = -1000000000;
            int tx = -1, ty = -1;
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (ChessBoard[i, j] != 0) continue;
                    ChessBoard[i, j] = 2;
                    if (judgeWin(i, j, 2)) 
                    {
                        ChessBoard[i, j] = 0;
                        return new Point(i, j);
                    }
                    long temp = CalValue(i, j, 2) + CalValue(i, j, 1) - FindMax();
                    if (temp > minr)
                    {
                        tx = i;
                        ty = j;
                        minr = temp;
                    }
                    ChessBoard[i, j] = 0;
                }
            }
            return new Point(tx, ty);
        }
        /// <summary>
        /// 当人下在x,y位置时返回PC下的位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point Play(int x, int y)
        {
            ChessBoard[x, y] = 1;
            Point ans = FindMin();
            ChessBoard[ans.x, ans.y] = 2;
            return ans;
        }
    }
    public partial class MainWindow : Window
    {
        private Wuziqi Scu_homework; //建立一个五子棋对象
        private bool isGameStart;  //判断游戏是否开始 true为开始 false为没有开始
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 根据Wuziqi类的棋盘来绘制现实的棋盘
        /// </summary>
        /// <param name="w"></param>
        public void DrawBoard(Wuziqi w)
        {
            Line[] xl = new Line[15];
            Line[] yl = new Line[15];
            myGrid.Children.Clear();
            for (int i = 0; i < 15; i++)
            {
                //MessageBox.Show(i.ToString());
                xl[i] = new Line();
                xl[i].Stroke = System.Windows.Media.Brushes.Black;
                xl[i].StrokeThickness = 2;
                xl[i].X1 = i * 40 + 10;
                xl[i].Y1 = 10;
                xl[i].X2 = i * 40 + 10;
                xl[i].Y2 = 570;
                myGrid.Children.Add(xl[i]);
                //MessageBox.Show("end" + i.ToString());
            }
            for (int i = 0; i < 15; i++)
            {
                yl[i] = new Line();
                yl[i].Stroke = System.Windows.Media.Brushes.Black;
                yl[i].StrokeThickness = 2;
                yl[i].X1 = 10;
                yl[i].Y1 = i * 40 + 10;
                yl[i].X2 = 570;
                yl[i].Y2 = i * 40 + 10;
                myGrid.Children.Add(yl[i]);
            }
            myGrid.Children.Add(myButton);
        }
        /// <summary>
        /// 画棋子用的函数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DrawQizi(double x, double y, int who)
        {
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush myBrush = new SolidColorBrush();
            if (who == 1)
            {
                myBrush.Color = Color.FromRgb(102, 204, 255);
            }
            else
            {
                myBrush.Color = Color.FromRgb(255, 255, 255);
            }
            myEllipse.Fill = myBrush;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = Brushes.Black;
            myEllipse.Width = 40;
            myEllipse.Height = 40;
            myEllipse.Margin = new Thickness(x - 20, y - 20, 800 - 20 - x, 700 - 20 - y);
            myGrid.Children.Add(myEllipse);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Scu_homework = new Wuziqi();
            DrawBoard(Scu_homework);
            isGameStart = false;
        }
        /// <summary>
        /// 开始按钮点击之后，应该启动一场新的游戏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            Scu_homework = new Wuziqi();
            DrawBoard(Scu_homework);
            isGameStart = true;
        }

        private void myGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isGameStart == false) return;
            double x = e.GetPosition(e.OriginalSource as FrameworkElement).X;
            double y = e.GetPosition(e.OriginalSource as FrameworkElement).Y;
            int fx = (int)(x + 10) / 40;
            int fy = (int)(y + 10) / 40;
            if (Scu_homework.JudgeBound(fx, fy) && Scu_homework.ChessBoard[fx, fy] == 0)
            {
                DrawQizi(40.0 * fx + 10, 40.0 * fy + 10, 1);
                Scu_homework.ChessBoard[fx, fy] = 1;
                if (Scu_homework.judgeWin(fx, fy, 1))
                {
                    isGameStart = false;
                    MessageBox.Show("you Win!");
                }
                else
                {
                    Point p = Scu_homework.Play(fx, fy);
                    DrawQizi(40.0 * p.x + 10, 40.0 * p.y + 10, 2);
                    if (Scu_homework.judgeWin(p.x, p.y, 2))
                    {
                        isGameStart = false;
                        MessageBox.Show("you Lose!");
                    }
                }
            }

        }

    }
}
