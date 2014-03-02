using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace Yahtzee
{
    public partial class Main : Form
    {
        int[] list = new int[] { };
        int[] count = new int[] { };        
        int[] backkeep = new int[] { };
        public System.Threading.Thread runner;
        public delegate void InvokeUpdate(bool[] kep, string hscore, string htype, int[] kcount);
        public delegate void InvokeScore(string best, double bests);
        public delegate void InvokeMethod();
        int rol = 1;

        int[] keep = new int[] { 0, 0, 0, 0, 0 };
        double upper_multiplier = 83; //83, 125, 200
        double kind_min = 15; //18, 15
        double digit_min = 15; //15
        string random_gen = "Pseudo"; //Crypto, Pseudo

        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static Random r = new Random();

        public Main()
        {
            InitializeComponent();
            Vers.Text = Application.ProductVersion;
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            string copy = fvi.LegalCopyright;
            Rights.Text = copy;

            //Aces.Visible = true;
            //Twos.Visible = true;
            //Threes.Visible = true;
            //Fours.Visible = true;
            //Fives.Visible = true;
            //Sixes.Visible = true;
            //Small.Visible = true;
            //Large.Visible = true;
            //House.Visible = true;
            //Kind3.Visible = true;
            //Kind4.Visible = true;
            //Chance.Visible = true;

            //Yahtzee.Visible = true;
            //Yahtzee.Text = "---";
            //B1.Visible = true;
            //B2.Visible = true;
            //B3.Visible = true;
        }
        
        private void Random_Click(object sender, EventArgs e)
        {
            Random.Enabled = false;            
            list = keep;
            Roll.Text = "Thinking...";
            for (int i = 0; i < 5; i++)
            {
                if (list[i] == 0)
                {
                    if (random_gen == "Crypto")
                    {
                        list[i] = RollDice((byte)6);
                    }
                    if (random_gen == "Pseudo")
                    {
                        list[i] = r.Next(1, 7);
                    }
                }
            }            
            if (rol == 1)
            {
                R1.Text = list[0].ToString();
                R2.Text = list[1].ToString();
                R3.Text = list[2].ToString();
                R4.Text = list[3].ToString();
                R5.Text = list[4].ToString();
                S1.Text = "?";
                S2.Text = "?";
                S3.Text = "?";
                S4.Text = "?";
                S5.Text = "?";
                U1.Text = "?";
                U2.Text = "?";
                U3.Text = "?";
                U4.Text = "?";
                U5.Text = "?";
                Label[] cl = new Label[] { R1, R2, R3, R4, R5, S1, S2, S3, S4, S5, U1, U2, U3, U4, U5};
                Microsoft.VisualBasic.PowerPacks.RectangleShape[] clr = 
                    new Microsoft.VisualBasic.PowerPacks.RectangleShape[] { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15};
                foreach (Label item in cl)
                {
                    item.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                }
                foreach (Microsoft.VisualBasic.PowerPacks.RectangleShape item in clr)
                {
                    item.BackColor = System.Drawing.SystemColors.ControlDarkDark;
                }
            }
            if (rol == 2)
            {
                S1.Text = list[0].ToString();
                S2.Text = list[1].ToString();
                S3.Text = list[2].ToString();
                S4.Text = list[3].ToString();
                S5.Text = list[4].ToString();
            }
            if (rol == 3)
            {
                U1.Text = list[0].ToString();
                U2.Text = list[1].ToString();
                U3.Text = list[2].ToString();
                U4.Text = list[3].ToString();
                U5.Text = list[4].ToString();
            }

            count = new int[] { 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < 5; i++)
            {
                count[Convert.ToInt32(list[i].ToString()) - 1] = count[Convert.ToInt32(list[i].ToString()) - 1] + 1;
            }
            keep = new int[] { 0, 0, 0, 0, 0 };
            rol++;
            if (rol == 4)
            {
                FinalDecide();                
            }
            else
            {
                Decide();
            }
        }

        public static byte RollDice(byte numberSides)
        {
            if (numberSides <= 0)
                throw new ArgumentOutOfRangeException("numberSides");

            // Create a byte array to hold the random value.
            byte[] randomNumber = new byte[1];
            do
            {
                // Fill the array with a random value.
                rngCsp.GetBytes(randomNumber);
            }
            while (!IsFairRoll(randomNumber[0], numberSides));
            // Return the random number mod the number 
            // of sides.  The possible values are zero- 
            // based, so we add one. 
            return (byte)((randomNumber[0] % numberSides) + 1);
        }

        private static bool IsFairRoll(byte roll, byte numSides)
        {
            // There are MaxValue / numSides full sets of numbers that can come up 
            // in a single byte.  For instance, if we have a 6 sided die, there are 
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete. 
            int fullSetsOfValues = Byte.MaxValue / numSides;

            // If the roll is within this range of fair values, then we let it continue. 
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use 
            // < rather than <= since the = portion allows through an extra 0 value). 
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair 
            // to use. 
            return roll < numSides * fullSetsOfValues;
        }

        private void Decide()
        {            
            //CURRENT ROLL priority (End turn if any of these crop up)
            //Yahtzee
            #region Yahtzee Test
            bool ytest = false;
            /*if (Yahtzee.Text != "---")
            {
                for (int i = 0; i < 5; i++)
                {
                    if (count[i] == 5)
                    {
                        if (Yahtzee.Visible == true)
                        {
                            if (B1.Visible == true)
                            {
                                if (B2.Visible == true)
                                {
                                    if (B3.Visible == true)
                                    {

                                    }
                                    else
                                    {
                                        B3.Visible = true;
                                        T3.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                                        TGrand.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                                        BPoint.Text = (Convert.ToInt32(BPoint.Text) + 100).ToString();
                                        ytest = true;
                                    }
                                }
                                else
                                {
                                    B2.Visible = true;
                                    T3.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                                    TGrand.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                                    BPoint.Text = (Convert.ToInt32(BPoint.Text) + 100).ToString();
                                    ytest = true;
                                }
                            }
                            else
                            {
                                B1.Visible = true;
                                T3.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                                TGrand.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                                BPoint.Text = (Convert.ToInt32(BPoint.Text) + 100).ToString();
                                ytest = true;
                            }

                        }
                        else
                        {
                            Yahtzee.Visible = true;
                            T3.Text = (Convert.ToInt32(T3.Text) + 50).ToString();
                            TGrand.Text = (Convert.ToInt32(T3.Text) + 50).ToString();
                            ytest = true;
                        }
                    }
                }
            }*/
            #endregion

            if (ytest == true)
            {
                
            }

            if (ytest == false)
            {
                //Start 120 thread processing
                global::Yahtzee.Properties.Settings.Default.BestScore.Clear();                
                global::Yahtzee.Properties.Settings.Default.BestType.Clear();                
                global::Yahtzee.Properties.Settings.Default.Ready.Clear();                

                for (int i = 0; i < 32; i++)
                {
                    global::Yahtzee.Properties.Settings.Default.BestScore.Add("-");                    
                    global::Yahtzee.Properties.Settings.Default.BestType.Add("-");                    
                    global::Yahtzee.Properties.Settings.Default.Ready.Add("0");                    
                }

                for (int i = 0; i < 32; i++)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(TestArrangement));                    
                    t.IsBackground = true;
                    t.Start(i);
                }

                runner = new System.Threading.Thread(WaitforThreads);
                runner.IsBackground = true;
                runner.Name = "WaitThread";
                runner.Start();

            }
        }

        private void WaitforThreads()
        {
            bool cont = true;
            while (cont)
            {
                try
                {
                    for (int i = 0; i < 32; i++)
                    {
                        if (global::Yahtzee.Properties.Settings.Default.Ready[i] == "0")
                        {
                            throw new Exception();
                        }
                    }
                    cont = false;
                }
                catch
                {

                }
            }
            //now, combine the results to get the confidence level and what digits to hold

            double hscore = -1;
            string htype = "NULL";
            int hid = -1;

            for (int i = 0; i < 32; i++)
            {
                if (Convert.ToDouble(global::Yahtzee.Properties.Settings.Default.BestScore[i]) > hscore)
                {
                    hscore = Convert.ToDouble(global::Yahtzee.Properties.Settings.Default.BestScore[i]);
                    htype = global::Yahtzee.Properties.Settings.Default.BestType[i];
                    hid = i;
                }
            }

            //get the 'hold' data in binary
            //then, output

            bool[] kep = new bool[] { false, false, false, false, false };
            List<int> kept = new List<int>();
            int c = hid;
            for (int i = 4; i > -1; i--)
            {
                if ((Math.Pow(2, i)) <= c)
                {
                    c = c - Convert.ToInt32((Math.Pow(2, i)));
                    kep[i] = true;
                    kept.Add(list[i]);
                }
            }
            int[] kcount = new int[] { 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < kept.Count; i++)
            {
                kcount[Convert.ToInt32(kept[i].ToString()) - 1] = kcount[Convert.ToInt32(kept[i].ToString()) - 1] + 1;
            }

                InvokeUpdate e = new InvokeUpdate(UpdateConfidence);
                Invoke(e, new object[] { kep, hscore.ToString(), htype, kcount });

            if (kept.Count == 5)
            {
                FinalDecide();
            }

        }

        private void TestArrangement(object id)
        {
            #region Initialization
            int id2 = (int)id;
            bool[] kep = new bool[] { false, false, false, false, false };
            List<int> kept = new List<int>();
            int c = id2;
            for (int i = 4; i > -1; i--)
            {
                if ((Math.Pow(2,i)) <= c)
                {
                    c = c - Convert.ToInt32((Math.Pow(2,i)));
                    kep[i] = true;
                    kept.Add(list[i]);
                }
            }
            int[] kcount = new int[] { 0, 0, 0, 0, 0, 0 };           
            for (int i = 0; i < kept.Count; i++)
            {
                kcount[Convert.ToInt32(kept[i].ToString()) - 1] = kcount[Convert.ToInt32(kept[i].ToString()) - 1] + 1;
            }
            #endregion
            string best = "NULL";
            double bests = -1;

            //int rol = current roll number + 1
            //int id2 = the id of the thread            
            //bool[] kep = whether each digit was kept or not

            //int[] kcount = amount of each number, 1-6
            //List<int> kept = all digits kept
            

            //NOW, go through every type of choice and figure out the chance!
            //THEN, subsidize scores that contain 2 or more of a number not used in the upper section
            //      and scores that, when chance is not used, give a score higher than twenty
            //      (for both, the closer and larger the score, the more the bonus)

            #region 3 of a kind
            bool exist = false;
            if (Kind3.Visible == true)
            {
                exist = true;
            }
            /*for (int i = 0; i < 6; i++)
            {
                if (kcount[i] >= 3)
                {
                    exist = true;
                }
            }*/
            if (exist == false)
            {                
                for (int i = 1; i < 7; i++)
                {
                    //check every digit
                    double diceneeded_kind = 3 - kcount[i - 1];
                    double turnsleft = 3 - (rol - 1);
                    double chance = (.17);
                    double diceneeded_rolled = 5 - kept.Count;
                    double currentscore = 0;
                    foreach (int j in kept)
                    {
                        currentscore = currentscore + j;
                    }
                    double maxscore = currentscore + (6 * diceneeded_rolled);
                    double score = (Math.Pow(chance, diceneeded_kind)) * maxscore;

                    bool[] scored = new bool[] { Aces.Visible, Twos.Visible, Threes.Visible
                            , Fours.Visible, Fives.Visible, Sixes.Visible};                    
                    if (scored[i - 1] == true)
                    {
                        score = score / 6;
                    }                    

                    if (score > bests)
                    {
                        bests = score;
                        best = "3 of a kind:" + i.ToString();
                    }                    
                }
            }
            #endregion

            #region 4 of a kind
            exist = false;
            if (Kind4.Visible == true)
            {
                exist = true;
            }
            /*for (int i = 0; i < 6; i++)
            {
                if (kcount[i] >= 4)
                {
                    exist = true;
                }
            }*/
            if (exist == false)
            {
                for (int i = 1; i < 7; i++)
                {
                    //check every digit
                    double diceneeded_kind = 4 - kcount[i - 1];
                    double turnsleft = 3 - (rol - 1);
                    double chance = (.17);
                    double diceneeded_rolled = 5 - kept.Count;
                    double currentscore = 0;
                    foreach (int j in kept)
                    {
                        currentscore = currentscore + j;
                    }
                    double maxscore = currentscore + (6 * diceneeded_rolled);
                    double score = (Math.Pow(chance, diceneeded_kind)) * maxscore;
                    bool[] scored = new bool[] { Aces.Visible, Twos.Visible, Threes.Visible
                            , Fours.Visible, Fives.Visible, Sixes.Visible};
                    if (scored[i - 1] == true)
                    {
                        score = score / 6;
                    } 
                    if (score > bests)
                    {
                        bests = score;
                        best = "4 of a kind:" + i.ToString();
                    }
                }
            }
            #endregion

            #region Small Straight
                exist = false;                
                if (Small.Visible == true)
                {
                    exist = true;
                }

                /*for (int i = 0; i < 3; i++)
                {
                    if ((kcount[i] > 0) && (kcount[i + 1] > 0) && (kcount[i + 2] > 0) && (kcount[i + 3] > 0))
                    {
                        exist = true;
                    }
                }*/
                if (exist == false)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        double missing = 0;
                        for (int j = i; j < i + 4; j++)
                        {
                            if (kcount[j - 1] < 1)
                            {
                                missing = missing + 1;
                            }
                        }
                        double chance = 1 / (Math.Pow(6, missing));
                        double score = chance * 30;
                        if (score > bests)
                        {
                            bests = score;
                            best = "Small Straight:" + i.ToString();
                        }
                    }
                }
            #endregion

            #region Large Straight
                exist = false;
                if (Large.Visible == true)
                {
                    exist = true;
                }

                /*for (int i = 0; i < 2; i++)
                {
                    if ((kcount[i] > 0) && (kcount[i + 1] > 0) && (kcount[i + 2] > 0) && (kcount[i + 3] > 0) && (kcount[i + 4] > 0))
                    {
                        exist = true;
                    }
                }*/
                if (exist == false)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        double missing = 0;
                        for (int j = i; j < i + 5; j++)
                        {
                            if (kcount[j - 1] < 1)
                            {
                                missing = missing + 1;
                            }
                        }
                        double chance = 1 / (Math.Pow(6, missing));
                        double score = chance * 40;
                        if (score > bests)
                        {
                            bests = score;
                            best = "Large Straight:" + i.ToString();
                        }
                    }
                }
            #endregion

            #region Full House
                exist = false;
                if (House.Visible == true)
                {
                    exist = true;
                }

            //Full house currently prioritized; if you get a full house, you keep the full house.
            //Add a full house checker to de-prioritize

                if (exist == false)
                {
                    for (int x = 1; x < 7; x++)
                    {
                        for (int y = 1; y < 7; y++)
                        {
                            if ((kcount[x - 1] < 4) && (kcount[y - 1] < 3) && (x != y))
                            {
                                double chance = 1 / (Math.Pow(6, (3 - kcount[x - 1]) + (2 - kcount[y - 1])));
                                double score = chance * 25;
                                if (score == 25)
                                {
                                    score = 50;
                                }
                                if (score > bests)
                                {
                                    bests = score;
                                    best = "Full House:" + x.ToString() + y.ToString();
                                }                                
                            }
                        }
                    }



                }
            #endregion

            #region Yahtzee
                exist = false;
                if (Yahtzee.Text == "---")
                {
                    exist = true;
                }
                else
                {
                    if (Yahtzee.Visible == true && B1.Visible == true && B2.Visible == true
                        && B3.Visible == true)
                    {
                        exist = true;
                    }
                }
                if (exist == false)
                {
                    for (int i = 1; i < 7; i++)
                    {
                        //check every digit
                        double diceneeded_kind = 5 - kcount[i - 1];
                        double turnsleft = 3 - (rol - 1);
                        double chance = (.17);
                        double diceneeded_rolled = 5 - kept.Count;
                        double currentscore = 0;
                        foreach (int j in kept)
                        {
                            currentscore = currentscore + j;
                        }
                        double maxscore = 10;
                        double score = (Math.Pow(chance, diceneeded_kind)) * maxscore;
                        //subsidize yahtzees where i is open in the upper section
                        bool[] scored = new bool[] { Aces.Visible, Twos.Visible, Threes.Visible
                            , Fours.Visible, Fives.Visible, Sixes.Visible};
                        if (best == "NULL" || best.StartsWith("Yahtzee:") )
                        {
                            if (scored[i - 1] == false)
                            {
                                score = score + 8.5;
                            }
                            else
                            {
                                score = score / 2;
                            }
                        }
                        if (score > bests)
                        {
                            bests = score;
                            best = "Yahtzee:" + i.ToString();
                        }
                    }
                }
            #endregion

            #region Chance

                if ((best == "NULL"))
                {
                    if (Chance.Visible == false)
                    {                        
                        double diceneeded_rolled = 5 - kept.Count;
                        double currentscore = 0;
                        foreach (int j in kept)
                        {
                            currentscore = currentscore + j;
                        }
                        double score = (currentscore) + (3.5 * diceneeded_rolled);
                        score = (score / 100);

                        if (score > bests)
                        {
                            bests = score;
                            best = "Chance:";
                        }
                    }
                }

            #endregion

            #region Digits 1-6
                            
                    for (int x = 1; x < 7; x++)
                    {
                        exist = false;
                        if ((x == 1) && (Aces.Visible == true))
                        {
                            exist = true;
                        }
                        if ((x == 2) && (Twos.Visible == true))
                        {
                            exist = true;
                        }
                        if ((x == 3) && (Threes.Visible == true))
                        {
                            exist = true;
                        }
                        if ((x == 4) && (Fours.Visible == true))
                        {
                            exist = true;
                        }
                        if ((x == 5) && (Fives.Visible == true))
                        {
                            exist = true;
                        }
                        if ((x == 6) && (Sixes.Visible == true))
                        {
                            exist = true;
                        }
                        if (exist == false)
                        {
                            double score = kcount[x - 1] * x;
                            score = 10 * (score / 63);
                            if (score > bests)
                            {
                                bool allow = true;

                                if ((x > 4) && (score < 20))
                                {
                                    if (best == "NULL")
                                    {
                                        allow = true;
                                    }
                                    else
                                    {
                                        allow = false;
                                    }
                                }
                                
                                if (allow == true)
                                {
                                    bests = score;
                                    best = "Digits:" + x.ToString();
                                }
                            }
                        }
                    }
                
                
            #endregion                          

                    //  CHANGE OF STRATEGY:
                    //  when all of the lower section is complete, all that's left is Chance and Digits
                    //  SAVE CHANCE FOR LAST!

                    //  unless...
                    //  if fives or sixes, must be at least 20 pts.

            //double bes = bests;
            /*if ((best != "Chance:") && (!(best.StartsWith("Digits:"))) && (best != "NULL"))
            {
                bes = Subsidize(kcount, bests, id2);
            }*/
            global::Yahtzee.Properties.Settings.Default.BestScore[id2] = bests.ToString();
            global::Yahtzee.Properties.Settings.Default.BestType[id2] = best;
            global::Yahtzee.Properties.Settings.Default.Ready[id2] = "1";
            return;
        }

        private double Subsidize(int[] kcount, double score, int id2)
        {
            double scoreb = score;
            bool[] scored = new bool[] { Aces.Visible, Twos.Visible, Threes.Visible
            , Fours.Visible, Fives.Visible, Sixes.Visible};

            //subsidize scores that contain 2 or more of a number not used in the upper section 
            double hscore = score;
            for (int i = 0; i < 6; i++)
            {
                if ((scored[i] == false) && (kcount[i] > 1))
                {
                    int amount = kcount[i];
                    double bonus = amount * (i + 1);
                    double bas = (bonus / 63);
                    bas = 1 + bas;
                    double scor = score * bas;
                    if (scor > hscore)
                    {
                        hscore = scor;
                    }
                }
            }

            score = hscore;

            /*double dice_left = 5;
            double cur_score = 0;

            //subsidize scores that, when chance is not used, give a score higher than 18
            for (int i = 0; i < 6; i++)
            {
                dice_left = dice_left - kcount[i];
                cur_score = cur_score + ((i + 1) * kcount[i]);
            }

            double pot_score = cur_score + (3.5 * dice_left);
            if ((Chance.Visible == false) && (pot_score > 18))
            {
                double bonus = 1 + (.1 * (Math.Pow(1.02, pot_score)));
                score = score * bonus;
            }*/

            return score;
        }

        private void FinalDecide()
        {

        #region Initialization

            int[] kcount = new int[] { 0, 0, 0, 0, 0, 0 };
            List<int> lists = list.ToList<int>();
            for (int i = 0; i < lists.Count; i++)
            {
                kcount[Convert.ToInt32(lists[i].ToString()) - 1] = kcount[Convert.ToInt32(lists[i].ToString()) - 1] + 1;
            }

        #endregion

            string best = "NULL";
            double bests = -1;

            //int[] kcount = amount of each number, 1-6
            //List<int> lists = all digits
            
            //First item in list is highest priority, last is lowest
            //If scores are equal, highest priority gets it
            
        #region Yahtzee
            bool exist = false;
            
            for (int i = 0; i < 6; i++)
            {
                if (kcount[i] == 5)
                {
                    exist = true;
                }
            }

            if (Yahtzee.Visible == true && B1.Visible == true && B2.Visible == true
                        && B3.Visible == true)
            {
                exist = false;
            }

            if (exist == true)
            {
                if ((B1.Visible == false) || (B2.Visible == false) || (B3.Visible == false))
                {
                    double score;
                    if (Yahtzee.Visible == true)
                    {
                        score = 100;
                    }
                    else
                    {
                        score = 50;
                    }

                    if (score > bests)
                    {
                        bests = score;
                        best = "Yahtzee";
                    }
                }
            }

       #endregion

        #region 4 of a Kind
            exist = false;

            for (int i = 0; i < 6; i++)
            {
                if (kcount[i] > 3)
                {
                    exist = true;
                }
            }

            if (Kind4.Visible == true)
            {
                exist = false;
            }

            if (exist == true)
            {
                double score = 0;
                for (int i = 0; i < 5; i++)
                {
                    score = score + lists[i];
                }

                if ((score > bests) && (score > kind_min))
                {
                    bests = score;
                    best = "4 of a Kind";
                }                
            }

        #endregion

        #region Full House
            exist = false;

            for (int x = 1; x < 7; x++)
            {
                for (int y = 1; y < 7; y++)
                {
                    if ((kcount[x - 1] == 3) && (kcount[y - 1] == 2) && (x != y))
                    {
                        exist = true;
                    }
                }
            }

            if (House.Visible == true)
            {
                exist = false;
            }

            if (exist == true)
            {
                double score = 25;

                if (score > bests)
                {
                    bests = score;
                    best = "Full House";
                }
            }

        #endregion

        #region 3 of a Kind
            exist = false;

            for (int i = 0; i < 6; i++)
            {
                if (kcount[i] > 2)
                {
                    exist = true;
                }
            }

            if (Kind3.Visible == true)
            {
                exist = false;
            }

            if (exist == true)
            {
                double score = 0;
                for (int i = 0; i < 5; i++)
                {
                    score = score + lists[i];
                }

                if ((score > bests) && (score > kind_min))
                {
                    bests = score;
                    best = "3 of a Kind";
                }
            }

        #endregion

        #region Large Straight
            exist = false;

            for (int i = 0; i < 2; i++)
                {
                    if ((kcount[i] > 0) && (kcount[i + 1] > 0) && (kcount[i + 2] > 0) && (kcount[i + 3] > 0) && (kcount[i + 4] > 0))
                    {
                        exist = true;
                    }
                }

            if (Large.Visible == true)
            {
                exist = false;
            }

            if (exist == true)
            {
                double score = 40;

                if (score > bests)
                {
                    bests = score;
                    best = "Large Straight";
                }
            }

        #endregion

        #region Small Straight
            exist = false;

            for (int i = 0; i < 3; i++)
            {
                if ((kcount[i] > 0) && (kcount[i + 1] > 0) && (kcount[i + 2] > 0) && (kcount[i + 3] > 0))
                {
                    exist = true;
                }
            }

            if (Small.Visible == true)
            {
                exist = false;
            }

            if (exist == true)
            {
                double score = 30;

                if (score > bests)
                {
                    bests = score;
                    best = "Small Straight";
                }
            }

        #endregion
            
        #region Digits 1-6
            double hcount = -1;
            int available_count = 0;
            int upperscore = Convert.ToInt32(T4.Text);

            for (int x = 1; x < 7; x++)
            {
                exist = true;
                if ((x == 1) && (Aces.Visible == true))
                {
                    exist = false;
                }
                if ((x == 2) && (Twos.Visible == true))
                {
                    exist = false;
                }
                if ((x == 3) && (Threes.Visible == true))
                {
                    exist = false;
                }
                if ((x == 4) && (Fours.Visible == true))
                {
                    exist = false;
                }
                if ((x == 5) && (Fives.Visible == true))
                {
                    exist = false;
                }
                if ((x == 6) && (Sixes.Visible == true))
                {
                    exist = false;
                }
                if (exist == true)
                {
                    available_count++;
                }
            }

            for (int x = 1; x < 7; x++)
            {
                exist = false;

                if (kcount[x - 1] > 0)
                {
                    exist = true;
                }

                if ((x == 1) && (Aces.Visible == true))
                    {
                       exist = false;
                    }
                if ((x == 2) && (Twos.Visible == true))
                    {
                        exist = false;
                    }
                if ((x == 3) && (Threes.Visible == true))
                    {
                        exist = false;
                    }
                if ((x == 4) && (Fours.Visible == true))
                    {
                        exist = false;
                    }
                if ((x == 5) && (Fives.Visible == true))
                    {
                        exist = false;
                    }
                if ((x == 6) && (Sixes.Visible == true))
                    {
                        exist = false;
                    }

                if (exist == true)
                {
                    double count = kcount[x - 1];
                    double amount = count * x;
                    double scr = (amount / 63);
                    double score = upper_multiplier * scr;
                    double min_score = digit_min / 63;
                    min_score = min_score * upper_multiplier;

                    if ((x > 4) && (score < min_score))
                    {
                        score = -2;
                    }

                    if (available_count == 1)
                    {
                        /*if ((upperscore + (4 * x)) < 63)
                        {
                            score = -2;
                        }
                        if (((upperscore + (3 * x)) < 63) && (score != -2))
                        {
                            score = -2;
                        }*/
                        if ((upperscore + amount < 63) && ((upperscore + (4*x)) >= 63))
                        {
                            //if not achieved and possible
                            score = -2;
                       }
                        
                    }

                    if ((score > bests) && (count > hcount))
                    {                       
                        bests = score;
                        best = "Digit " + x.ToString();
                        hcount = count;
                    }                    
                }
            }            

        #endregion

        #region Chance
            exist = false;

            if ((best == "NULL") && (Chance.Visible == false))
            {
                exist = true;
            }

            if (exist == true)
            {
                double score = 0;
                for (int i = 0; i < 5; i++)
                {
                    score = score + lists[i];
                }

                if (score > bests)
                {
                    bests = score;
                    best = "Chance";
                }
            }

        #endregion

        #region Digits 1-6 (Last Resort)
            if (best == "NULL")
            {
                hcount = -1;
                for (int x = 1; x < 7; x++)
                {
                    exist = false;

                    if (kcount[x - 1] > 0)
                    {
                        exist = true;
                    }

                    if ((x == 1) && (Aces.Visible == true))
                    {
                        exist = false;
                    }
                    if ((x == 2) && (Twos.Visible == true))
                    {
                        exist = false;
                    }
                    if ((x == 3) && (Threes.Visible == true))
                    {
                        exist = false;
                    }
                    if ((x == 4) && (Fours.Visible == true))
                    {
                        exist = false;
                    }
                    if ((x == 5) && (Fives.Visible == true))
                    {
                        exist = false;
                    }
                    if ((x == 6) && (Sixes.Visible == true))
                    {
                        exist = false;
                    }

                    if (exist == true)
                    {
                        double count = kcount[x - 1];
                        double amount = count * x;
                        amount = (amount / 63);
                        double score = upper_multiplier * amount;
                        
                        if ((score > bests) && (count > hcount))
                        {
                            bests = score;
                            best = "Digit " + x.ToString();
                            hcount = count;
                        }
                    }
                }    
            }

            #endregion
            
            //Cross-off (Absolute last resort)
            //If still null, cross something off!

        #region Cross-off (Absolute last resort)

            if (best == "NULL")
            {
                best = "Cross Off!";
            }

        #endregion

            //Laster resorts
            //3-4 of a kind, where score is less than 16 will not go, unless
            //it hits here...

        #region 4 of a Kind (laster resort)

            if (best == "Cross Off!")
            {
                exist = false;

                for (int i = 0; i < 6; i++)
                {
                    if (kcount[i] > 3)
                    {
                        exist = true;
                    }
                }

                if (Kind4.Visible == true)
                {
                    exist = false;
                }

                if (exist == true)
                {
                    double score = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        score = score + lists[i];
                    }

                    if (score > bests)
                    {
                        bests = score;
                        best = "4 of a Kind";
                    }
                }
            }

        #endregion

        #region 3 of a Kind (laster resort)
            if (best == "Cross Off!")
            {
                exist = false;

                for (int i = 0; i < 6; i++)
                {
                    if (kcount[i] > 2)
                    {
                        exist = true;
                    }
                }

                if (Kind3.Visible == true)
                {
                    exist = false;
                }

                if (exist == true)
                {
                    double score = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        score = score + lists[i];
                    }

                    if (score > bests)
                    {
                        bests = score;
                        best = "3 of a Kind";
                    }
                }
            }

        #endregion

            rol = 1;
            keep = new int[] { 0, 0, 0, 0, 0 };
                        
            InvokeScore f = new InvokeScore(UpdateScore);
            Invoke(f, new object[] { best, bests });
            
        }

        private void UpdateScore(string best, double bests)
        {

            #region Update Confidence
            Q2.Text = best;
            Q3.Text = "100%";
            W1.Width = 171;
            W1.BackColor = Color.FromArgb(0, 192, 192);
            #endregion

            #region Yahtzee
            if (best == "Yahtzee")
            {
                if (Yahtzee.Visible == true)
                {
                    if (B1.Visible == true)
                    {
                        if (B2.Visible == true)
                        {
                            if (B3.Visible == true)
                            {

                            }
                            else
                            {
                                BPoint.Text = (Convert.ToInt32(BPoint.Text) + 100).ToString();
                                B3.Visible = true;
                                T3.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                                TGrand.Text = (Convert.ToInt32(TGrand.Text) + 100).ToString();
                            }
                        }
                        else
                        {
                            BPoint.Text = (Convert.ToInt32(BPoint.Text) + 100).ToString();
                            B2.Visible = true;
                            T3.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                            TGrand.Text = (Convert.ToInt32(TGrand.Text) + 100).ToString();
                        }
                    }
                    else
                    {
                        BPoint.Text = (Convert.ToInt32(BPoint.Text) + 100).ToString();
                        B1.Visible = true;
                        T3.Text = (Convert.ToInt32(T3.Text) + 100).ToString();
                        TGrand.Text = (Convert.ToInt32(TGrand.Text) + 100).ToString();
                    }
                }
                else
                {
                    Yahtzee.Visible = true;
                    T3.Text = (Convert.ToInt32(T3.Text) + 50).ToString();
                    TGrand.Text = (Convert.ToInt32(TGrand.Text) + 50).ToString();
                }
            }
            #endregion

            #region Lower Section
            if (best == "3 of a Kind")
            {
                Kind3.Visible = true;
                Kind3.Text = bests.ToString();
                T3.Text = (Convert.ToInt32(T3.Text) + bests).ToString();
                TGrand.Text = (Convert.ToInt32(TGrand.Text) + bests).ToString();
            }

            if (best == "4 of a Kind")
            {
                Kind4.Visible = true;
                Kind4.Text = bests.ToString();
                T3.Text = (Convert.ToInt32(T3.Text) + bests).ToString();
                TGrand.Text = (Convert.ToInt32(TGrand.Text) + bests).ToString();
            }

            if (best == "Full House")
            {
                House.Visible = true;
                House.Text = "25";
                T3.Text = (Convert.ToInt32(T3.Text) + 25).ToString();
                TGrand.Text = (Convert.ToInt32(TGrand.Text) + 25).ToString();
            }

            if (best == "Small Straight")
            {
                Small.Visible = true;
                Small.Text = "30";
                T3.Text = (Convert.ToInt32(T3.Text) + 30).ToString();
                TGrand.Text = (Convert.ToInt32(TGrand.Text) + 30).ToString();
            }

            if (best == "Large Straight")
            {
                Large.Visible = true;
                Large.Text = "40";
                T3.Text = (Convert.ToInt32(T3.Text) + 40).ToString();
                TGrand.Text = (Convert.ToInt32(TGrand.Text) + 40).ToString();
            }

            if (best == "Chance")
            {
                Chance.Visible = true;
                Chance.Text = bests.ToString();
                T3.Text = (Convert.ToInt32(T3.Text) + bests).ToString();
                TGrand.Text = (Convert.ToInt32(TGrand.Text) + bests).ToString();
            }

            #endregion

            #region Upper Section
            if (best.StartsWith("Digit "))
            {
                int digit = Convert.ToInt32(best.Substring(6, 1));
                Label[] locs = new Label[] { Aces, Twos, Threes, Fours, Fives, Sixes };

                double score = (bests * 63);
                score = (score / upper_multiplier);

                locs[digit - 1].Text = score.ToString();
                locs[digit - 1].Visible = true;
                T1.Text = (Convert.ToInt32(T1.Text) + score).ToString();
                if ((Convert.ToInt32(T1.Text) >= 63) && (Bonus.Visible == false))
                {
                    score = score + 35;
                    Bonus.Visible = true;
                }

                T2.Text = (Convert.ToInt32(T2.Text) + score).ToString();
                T4.Text = (Convert.ToInt32(T4.Text) + score).ToString();
                TGrand.Text = (Convert.ToInt32(TGrand.Text) + score).ToString();
            }
            #endregion

            #region Cross-Offs (Absolute last resort)
            
                if (best == "Cross Off!")
                {
                    Label[] order = new Label[] { Yahtzee, Large, Small, Aces, Twos, Threes, Kind4, House, Fours, Kind3, Fives, Sixes};
                    bool crossed = false;
                    for (int i = 0; i < order.Length; i++)
                    {
                        if ((order[i].Visible == false) && (crossed == false))
                        {
                            order[i].Visible = true;
                            order[i].Text = "---";
                            crossed = true;
                            if (i == 0)
                            {
                                B1.Visible = true;
                                B2.Visible = true;
                                B3.Visible = true;
                                B1.Text = " ";
                                B2.Text = " ";
                                B3.Text = " ";
                            }
                        }
                    }
                }            

            #endregion

            #region Check End

                Label[] check = new Label[] { Aces, Twos, Threes, Fours, Fives, Sixes, Kind3, Kind4, House, Small, Large, Yahtzee, Chance};
                bool end = true;
                foreach (Label item in check)
                {
                    if (item.Visible == false)
                    {
                        end = false;
                    }
                }
                if (end == true)
                {
                    Roll.Text = "Game Over!";
                    Random.Enabled = false;
                    return;
                }

            #endregion

            Roll.Text = "Roll 1";
            Random.Enabled = true;
        }

        private void UpdateConfidence(bool[] kep, string hscore, string htype, int[] kcount)
        {
            //Update confidence display and digit hold display here

            Label[] cl = new Label[] { R1, R2, R3, R4, R5, S1, S2, S3, S4, S5, U1, U2, U3, U4, U5 };
            Microsoft.VisualBasic.PowerPacks.RectangleShape[] clr =
                new Microsoft.VisualBasic.PowerPacks.RectangleShape[] { d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13, d14, d15 };
            keep = new int[] { 0, 0, 0, 0, 0 };
            for (int i = 0; i < 5; i++)
			{
                int j = ((5 * (rol - 1)) - 5) + i;
                if (kep[i] == true)
                {
                    keep[i] = list[i];
                    cl[j].BackColor = System.Drawing.SystemColors.Highlight;
                    clr[j].BackColor = System.Drawing.SystemColors.Highlight;
                }
			}
            double scor = 0;
            try
            {
                double a = 54.528414604039;
                double b = 11.623547543781;
                double score = a + (b * (Math.Log(Convert.ToDouble(hscore))));
                scor = Math.Floor(score);
                if (scor > 0)
                {
                    Q3.Text = scor.ToString() + "%";
                }
                else
                {
                    Q3.Text = "0%";
                }
                int wid = Convert.ToInt32((scor / 100) * 171);

                if (wid < 5)
                {
                    wid = 5;
                }
                W1.Width = wid;
                W1.Visible = true;
            }
            catch
            {
                Q3.Text = "0%";
                W1.Width = 3;
                W1.Visible = false;
            }
            
            if (scor < 55)
            {
                W1.BackColor = Color.Red;
            }
            else
            {
                if (scor > 69)
                {
                    W1.BackColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    W1.BackColor = Color.FromArgb(192, 192, 0);
                }
            }

            Q2.Text = htype.Substring(0, htype.IndexOf(":"));
            Random.Enabled = true;
            Roll.Text = "Roll " + rol;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }
}
