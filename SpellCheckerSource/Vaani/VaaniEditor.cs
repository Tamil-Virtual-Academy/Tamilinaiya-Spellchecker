using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;//streamreader
using Newtonsoft.Json;//jsonconvert
using System.Globalization;//CharUnicodeInfo

#region namespace used for verifing the word with algorithm to check if is correct or wrong

namespace VaaniEditor
{
    public class Vaani
    {

        string path = "";//for WebBrowser convenience

        dynamic db = JsonConvert.DeserializeObject(Vaani.getDB(@"koppu/DB.json"));//bit quicker than reading in gpathil10


        string peyar = "MLTYWNEIQOGDHVXBPSളവ";
        string speyar = "CAJ";//C is special permit without 15
        string venai = "ஆலனசளணஇழஉஓடதदधபநमயரறवउपकतईटरलळएचज";
        string nonderi = "Z";//JRJR spare
        string deri;//it doesn't recognize peyar as string, so using constructor we have concated the string
        public Vaani() { deri = peyar + speyar + venai + "FUKഡഗജദപ"; }
        dynamic Oword;
        dynamic Eword;
        dynamic tranrule;
        dynamic tword;
        dynamic gword;
        dynamic vauyir;
        dynamic yauyir;
        dynamic auyir;
        string[] userOword;
        string[] usergword;
        //To cache the words and suggestion till the software closed
        List<string> cacheword = new List<string>();//HashSet is not used since cachesug has duplicate
        List<string> cachesug = new List<string>();
        public void refreshcache(string nword)
        {
            //userOword=userOword.Concat(new []{nword}).ToArray();
            //cacheword.Add(nword);
            //cachesug.Add("correct");
            int found = cacheword.IndexOf(nword);//to add cache data programatically when user needs
            cachesug[found] = "correct";
        }
#region check the word with existing root words
        public dynamic checkroot(string tword)//morphgen
        {
            string[] outp = "tword,".Split(',');
            Oword = db["DB"][4];//Words
            Eword = db["DB"][3];//family
            dynamic rule = JsonConvert.DeserializeObject("{\"M\":\"ம்\",\"L\":\"ு\",\"T\":\"ு\",\"Y\":\"\",\"W\":\"\",\"N\":\"\",\"E\":\"\",\"I\":\"ல்\",\"Q\":\"ள்\",\"ള\":\"ள்\",\"O\":\"்\",\"P\":\"்\",\"S\":\"்\",\"V\":\"ு\",\"വ\":\"ு\",\"ഗ\":\"\",\"ഡ\":\"\",\"ജ\":\"\",\"ദ\":\"\",\"പ\":\"\",\"B\":\"ை\",\"G\":\"ர்\",\"D\":\"ர்\",\"X\":\"ர்\",\"H\":\"ர்\",\"ஆ\":\"தல்\",\"ல\":\"ல்தல்\",\"ன\":\"ல்தல்\",\"ச\":\"்லுதல் \",\"ள\":\"ள்தல்\",\"ண\":\"ள்ளுதல்\",\"இ\":\"ுதல்\",\"ழ\":\"ுதல்\",\"உ\":\"தல்\",\"ஓ\":\"தல்\",\"ட\":\"ுதல்\",\"த\":\"த்தல்\",\"द\":\"த்தல்\",\"ध\":\"த்தல்\",\"ப\":\"்த்தல்\",\"ந\":\"த்தல்\",\"म\":\"த்தல்\",\"ய\":\"தல்\",\"ர\":\"்தல்\",\"ற\":\"ுதல்\",\"व\":\"ாதல்\",\"उ\":\"ணுதல்\",\"प\":\"்ணுதல்\",\"क\":\"ாண்ணுதல்\",\"त\":\"னுதல்\",\"ई\":\"்னுதல்\",\"ट\":\"ள்தல்\",\"र\":\"ல்தல்\",\"ल\":\"ல்தல்\",\"ळ\":\"ுதல்\",\"ए\":\"றுதல்\",\"च\":\"தல் \",\"ज\":\"ேகுதல்\"}");

            for (int a = tword.Length; a > 0; a--)
            {//It cuts from end. end is best since small word has many derivation like ഊ
                //for(int a=1;a<=sol.length;a++){//it cuts from starting
                string paku = tword.Substring(0, a);
                string viku = tword.Substring(a);//,sol.length
                dynamic qcode = Oword[paku];
                if (qcode != null) if (qcode.Count > 0) foreach (dynamic b in qcode)
                        {
                            if (b.s != null) return false;//to save time for foreign words and misspelled words
                            string code = ((string)b.t).Substring(0, 1);
                            string subcode = ((string)b.t).Substring(1);
                            string vikuthi = getviku(viku, code, subcode);
                            if (vikuthi != "false") { outp[0] = paku + (string)rule[code]; outp[1] = " " + tranlate(vikuthi); return outp; }

                        }
            }
            return outp;
        }
#endregion

#region give suffix words
        public string getviku(string v, string c, string sc)//used for dictionary
        {
            string blocks = "";
            switch (sc)
            {
                case "15": blocks = "01234567"; break;
                case "25": blocks = "0123456"; break;
                case "07":
                    blocks = "012345"; break;
                case "10"://
                    blocks = "01235"; break;
                case "11"://except special 
                    blocks = "012356"; break;
                case "09":
                    blocks = "02"; break;
                case "06":
                    blocks = "023"; break;
                case "05":
                    blocks = "013"; break;
                case "04":
                    blocks = "03"; break;
                case "03":
                    blocks = "13"; break;
                case "02":
                    blocks = "2"; break;
                case "01":
                    blocks = "1"; break;
                case "16":
                    blocks = "3"; break;
                case "17"://special
                    blocks = "0"; break;
                case "08"://Peyar speical extension அநவரத  venai present echam சுவைப்பட,நாள்பட 
                    blocks = "4"; break;
                case "18"://special  -> 08 also takes 4 segment
                    blocks = "4"; break;
                case "19"://special
                    blocks = "5"; break;
                case "20"://special
                    blocks = "6"; break;
                case "21"://special
                    blocks = "7"; break;
                case "24"://echam verb is using
                    blocks = "34"; break;
            }


            for (var d = 0; d < 8; d++)
            {
                if (blocks.IndexOf(Convert.ToString(d)) == -1) continue;//it will ignore the blocks which are not related
                //Logger.log(Eword[c][d][v]);
                //if(Eword[c][d][v]!=null){return Eword[c][d][v];}
                for (var b = v.Length; b > -1; b--)
                {

                    string subpaku = v.Substring(0, b);
                    string subviku = v.Substring(b);
                    //Logger.log(subpaku +" " +subviku + " c="+ c+ " d="+ d);
                    //if (subpaku == "") subpaku = "0";//since no need for c#
                    try
                    {
                        string part = (string)Eword[c][d][subpaku];
                        if (part != null)
                        {

                            //if derivative then go further else return as true
                            if ((part.IndexOf("①") != -1) || (part.IndexOf("②") != -1))
                            {

                                string code1 = part.Substring(part.Length - 3, 1);
                                string subcode1 = part.Substring(part.Length - 2);

                                string vikuth = getviku(subviku, code1, subcode1);

                                if (vikuth != "false") { return vikuth; }

                            }
                            else if (subviku == "") { return part; }

                        }
                    }
                    catch { }
                    //Logger.log(subviku+ " " + code +"  subviku="+ subcode);
                }
            }//end of d
            return "false";
        }
#endregion

#region get case names if case names are applicable
        public string tranlate(string code)
        {
            dynamic translation = JsonConvert.DeserializeObject("{\"㚱\":\"நான்காம் வேற்றுமை(கு)\",\"㚲\":\"இரண்டாம் வேற்றுமை(ஐ)\",\"㚳\":\"வேற்றுமை உருபு(இன்)\",\"㚵\":\"மூன்றாம் வேற்றுமை(உடன்)\",\"㚶\":\"மூன்றாம் வேற்றுமை(ஓடு)\",\"㚷\":\"மூன்றாம் வேற்றுமை(ஆல்)\",\"㚸\":\"வேற்றுமை உருபு(இல்)\",\"㚹\":\"ஏழாம் வேற்றுமை(இடம்)\",\"㚺\":\"ஆறாம் வேற்றுமை\"}");
            foreach (dynamic i in translation)
            {
                String d = Convert.ToString(i.Name);
                if (code.IndexOf(d) != -1)
                {
                    return "+ " + Convert.ToString(i.Value);
                }//,sw.Length-1
            }
            return "";
        }
#endregion

#region read Json file from path
        public static string getDB(string file)
        {
            //string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            //StreamReader r = new StreamReader(Path.Combine(path, file));
            try
            {
                StreamReader r = new StreamReader(file);
                string json = r.ReadToEnd();
                r.Close();
                r.Dispose();
                return json;
            }
            catch { return "{}"; }
        }
#endregion

#region validate the words and return result if it is correct words
        public dynamic gpathil11(string[] mword, Boolean opt, string mode)
        {
            /*
Order of the words are required to analyse 
Input (["என்த","", "சத்துக்", "","கள்", "", "சக்த்க்து"], true)
Output [[1, "எந்த"], [-1,""], [0, "correct"], [-1,""], [2, "குமாரே,குமாரு"], [-1,""], [0, "wrong"]]

             */
            //Original Code is Google apps script. input and Output format of C# remain same
            //var cache = ""; //local derivative cache
            if (false)//give true to run cache sheet, give false to run no cache for sheet 
            {
            }
            else
            {
                //StreamReader r1 = new StreamReader(@"C:\Users\raja_raman\Documents\Visual Studio 2013\Projects\Vaani\Vaani\DB.json");
                //string uri = "http://script.google.com/macros/s/AKfycbz3gO7EpKrVVaPjAP6NN39ERJf5zBj7XTy426AyrHo/dev?typ=feed";
                //string json =  new WebClient().DownloadString(uri);



                Oword = db["DB"][4];
                Eword = db["DB"][3];
                tranrule = db["DB"][2];
                tword = db["DB"][1];
                gword = db["DB"][0];
                StreamReader s = new StreamReader(path + @"koppu\User.Txt");
                string txt = s.ReadToEnd();
                txt = txt.Replace("\r\n", "\n");
                string[] user = txt.Split('\n');
                s.Close();
                s.Dispose();
                userOword = user[0].Split(',');
                usergword = user[1].Split(',');
            }

            //string result="";
            char[] splitchar = { ',' };
            // string[] sandhi = { "க", "ச", "த", "ப" };
            vauyir = JsonConvert.DeserializeObject("{\"வா\":\"ஆ\",\"வி\":\"இ\",\"வீ\":\"ஈ\",\"வு\":\"உ\",\"வூ\":\"ஊ\",\"வெ\":\"எ\",\"வே\":\"ஏ\",\"வை\":\"ஐ\",\"வொ\":\"ஒ\",\"வோ\":\"ஓ\",\"வௌ\":\"ஒள\"}");//\"வ\":\"அ\",
            yauyir = JsonConvert.DeserializeObject("{\"யா\":\"ஆ\",\"யி\":\"இ\",\"யீ\":\"ஈ\",\"யு\":\"உ\",\"யூ\":\"ஊ\",\"யெ\":\"எ\",\"யே\":\"ஏ\",\"யை\":\"ஐ\",\"யொ\":\"ஒ\",\"யோ\":\"ஓ\",\"யௌ\":\"ஒள\"}");// \"ய\":\"அ\",
            auyir = JsonConvert.DeserializeObject("{\"ா\":\"ஆ\",\"ி\":\"இ\",\"ீ\":\"ஈ\",\"ு\":\"உ\",\"ூ\":\"ஊ\",\"ெ\":\"எ\",\"ே\":\"ஏ\",\"ை\":\"ஐ\",\"ொ\":\"ஒ\",\"ோ\":\"ஓ\",\"ௌ\":\"ஒள\"}");// 


            dynamic[,] parinthu = new dynamic[mword.Length, 2];//size is fixed
            dynamic[,] ottran = new dynamic[mword.Length, 2];
            for (int i = 0; i < mword.Length; i++)
            {//parinthu[i]=new string[2];
                parinthu[i, 0] = 0;//count of suggestion
                parinthu[i, 1] = "wrong";//suggestions
                ottran[i, 0] = 0; ottran[i, 1] = 1;
            }
            /* ottran declaration
            [i][0]-> count of the array[i]. 1-correct or suggestion, 0-no
            [i][1]-> words or wrong or correct string
            Gcache concept- store all word results, check with new word when ever needed and do accordingly
            */

            //checking
            for (int i = 0; i < mword.Length; i++)//if(ottran[i][1]==1)
            {
                //give continue or ottran[i,0]=1
                //var start = new Date().getTime();
                string sandi = "";
                Boolean punarchi = false;
                //1 - if it is verified already
                if (ottran[i, 0] == 1) continue;
                //2 - removing blank char
                if (mword[i].Length < 1) { parinthu[i, 0] = -1; parinthu[i, 1] = ""; continue; }

                //3.ignoring single consonant letters
                if (mword[i].Length == 2)
                {
                    Regex rgx = new Regex(@"[ா-்]");
                    if (rgx.IsMatch(mword[i].Substring(mword[i].Length - 1)))
                    { ottran[i, 0] = 1; parinthu[i, 1] = "correct"; parinthu[i, 0] = 0; continue; }
                }
                //4.ignoring single vowel letters
                if (mword[i].Length == 1)
                { ottran[i, 0] = 1; parinthu[i, 1] = "correct"; parinthu[i, 0] = 0; continue; }

                //5- Typo Correction
                mword[i] = mword[i].Replace("ொ", "ொ");
                mword[i] = mword[i].Replace("ோ", "ோ");

                //6 - Translation
                if (opt == true) if (ottran[i, 0] == 0)
                    {
                        var istrans = false;
                        foreach (dynamic j in tword)
                        {
                            String tname = Convert.ToString(j.Name);
                            if (mword[i].Contains(tname) == true)
                            {
                                if (tword[tname].Count > 0) foreach (dynamic k in tword[tname])
                                    {//k is array of suggestions
                                        string a = Convert.ToString(k.t);
                                        string b = Convert.ToString(k.w);
                                        //Logger.log(b);
                                        foreach (dynamic l in tranrule[a])
                                        {
                                            string[] map = Convert.ToString(l.t).Split(',');
                                            if (mword[i].Contains(tname + map[0]) == true)
                                            {
                                                var nword = mword[i].Replace(tname + map[0], b + map[1]);//Logger.log(nword);
                                                if (checkword(nword, 0)) { addparinthu(parinthu, i, nword); istrans = true; }/*adding new parinthurai*/
                                            }
                                        }//end of l
                                    }//end of k
                            }//end of j
                        }
                        if (istrans == true) ottran[i, 0] = 1;
                    }

                //7.sandhi remover and sandi/punarchi memory
                if ((i + 2) < mword.Length) if (mword[i + 2].Length > 0)
                    {
                        string ottru = mword[i].Substring(mword[i].Length - 2);
                        string methi = mword[i].Substring(0, mword[i].Length - 2);
                        string muthal = mword[i + 2].Substring(0, 1);
                        Regex rgx1 = new Regex(@"[கசதப]்");
                        Regex rgx2 = new Regex(@"[கசதப]");
                        if (rgx1.IsMatch(ottru)) { if (muthal + "்" == ottru) { mword[i] = methi; sandi = ottru; } }
                        else if (ottru == "ட்") { if (rgx2.IsMatch(muthal)) { mword[i] = methi + "ள்"; punarchi = true; } }
                        else if (ottru == "ற்") { if (rgx2.IsMatch(muthal)) { mword[i] = methi + "ல்"; punarchi = true; } }
                        else if (ottru == "ங்") { if (muthal == "க") { mword[i] = methi + "ம்"; sandi = "ங்"; punarchi = true; } }
                        else if (ottru == "ஞ்") { if (muthal == "ச") { mword[i] = methi + "ம்"; sandi = "ஞ்"; punarchi = true; } }
                        else if (ottru == "ந்") { if (muthal == "த") { mword[i] = methi + "ம்"; sandi = "ந்"; punarchi = true; } }
                    }


                //8. skip if it is repeated word
                int found = cacheword.IndexOf(mword[i] + sandi);// for (int j = 0; j < cacheword.Count; j++)
                if (found > -1)
                {
                    string a = cacheword.ElementAt(found); if (a == mword[i] + sandi)
                    {
                        string b = cachesug.ElementAt(found); parinthu[i, 1] = b;
                        if (!istamil(b)) { parinthu[i, 0] = 0; }
                        else if (b.IndexOf(',') < 0) { parinthu[i, 0] = 1; }
                        else { parinthu[i, 0] = b.Split(',').Length; }
                        ottran[i, 0] = 1;
                        //break;
                    }
                }

                //9 - skip if was userpreferance
                if (ottran[i, 0] == 0) foreach (string a in userOword)
                    { if (a == mword[i].ToString()) { ottran[i, 0] = 1; parinthu[i, 1] = "correct"; parinthu[i, 0] = 0; } }

                if (ottran[i, 0] == 0) foreach (string a in usergword)
                    {
                        string[] nword = a.Split('|');
                        if (nword[0] == mword[i].ToString()) { parinthu = addparinthu(parinthu, i, nword[1]); }
                    }

                //10 - word match
                if (ottran[i, 0] == 0) if (checkword(mword[i], 0))
                    { ottran[i, 0] = 1; parinthu[i, 1] = "correct"; parinthu[i, 0] = 0; }


                //11 - gword suggestion
                if (opt == true) if (ottran[i, 0] == 0)
                    {
                        string[] sample = getsuggestion(mword[i]);
                        dynamic emp = JsonConvert.DeserializeObject("{}");
                        string[] sample2 = getsuggestion2(mword[i]);//, emp, 0
                        sample = sample.Concat(sample2).ToArray();
                        string[] usample = RemoveDuplicates(sample);//sample.filter(function(item, pos) {return sample.indexOf(item) == pos;})//to remove duplicate
                        //   if(sample.Length<1000)
                        foreach (dynamic l in usample)
                        {
                            string nword = l;
                            if (checkword(nword, 7))
                            {
                                if (punarchi)
                                {
                                    string ottru = nword.Substring(nword.Length - 2); string methi = nword.Substring(0, nword.Length - 2);
                                    if (ottru == "ள்") { addparinthu(parinthu, i, methi + "ட்"); } else if (ottru == "ல்") { addparinthu(parinthu, i, methi + "ற்"); } else if (ottru == "ம்") { addparinthu(parinthu, i, methi + sandi); };
                                }
                                else { parinthu = addparinthu(parinthu, i, nword + sandi); }
                            }
                        }
                    }//end of sugg



                //12 cache the search
                if (mword[i].Length > 0)
                {
                    if (!cacheword.Contains(mword[i] + sandi))
                    {
                        cacheword.Add(mword[i] + sandi);//sandi is added, since this parinthu have accordingly
                        cachesug.Add(parinthu[i, 1]);//cache all word
                    }
                } //memorize two array as string

                //13 - Check sandhi need or not needed should not cache

                if (ottran[i, 0] == 1)//if this word is correct
                {
                    if (mword.Length > i + 2) if (mword[i + 2].Length > 1)//if next word is available & that word should should not less than one character
                        {
                            string chandi = mword[i + 2].Substring(0, 1) + "்";//if user did give chandi
                            Regex rgx1 = new Regex(@"[கசதப]்");
                            if (rgx1.IsMatch(chandi))//if next word is kachathapa
                            {
                                //check next word and record the ottran to avoid recheck
                                if (checkword(mword[i + 2], 0)) { ottran[i + 2, 0] = 1; parinthu[i + 2, 1] = "correct"; parinthu[i + 2, 0] = 0; }
                                if (ottran[i + 2, 0] == 1)//if next word is correct
                                {
                                    bool combo = checkword(mword[i] + mword[i + 2], 0);
                                    bool thibo = checkword(mword[i] + chandi + mword[i + 2], 0);
                                    bool derive = checkword(mword[i], 5);//return true if it is valid perfect noun
                                    if (sandi != "")
                                    {
                                        if (combo && !thibo) { parinthu = addparinthu(parinthu, i, mword[i]); }
                                    }
                                    else//if no sandi
                                    {
                                        if (thibo && !combo && !derive) { parinthu = addparinthu(parinthu, i, mword[i] + chandi); }
                                    }
                                }

                            }
                        }
                }
                //14 - for Developer Sheet research
                if (ottran[i, 0] == 0) if (parinthu[i, 0] > 0)
                    {
                        //byproduct(mword[i], parinthu[i].join(",")); 
                    }

            }//end of i
            if (mode.Equals("web"))
            {//for string return
                string z = ":";
                string Arr = "";
                foreach (dynamic i in parinthu)//foreach can't be used, since dynamic(multi dimension) will return all units and no increments are not accepted
                {
                    Arr += Convert.ToString(i) + z;
                    if (z == ":") { z = "|"; } else { z = ":"; }
                }
                return Arr.Substring(0, Arr.Length - 1);
            }
            else
            {
                return parinthu;
            }
        }

#endregion

#region just remove the duplicates from array
        public static string[] RemoveDuplicates(string[] s)
        {
            HashSet<string> set = new HashSet<string>(s);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }
#endregion

#region get suggestions for given words
        public string[] getsuggestion(string c)//c is  mword[i]
        {
            string[] sug = { };
            foreach (dynamic j in gword)
            {//j gives every miswords
                String a = Convert.ToString(j.Name);
                if (c.Contains(a))
                {

                    if (gword[a].Count > 0) foreach (dynamic k in gword[a])
                        {//k is array of suggestions
                            string b = Convert.ToString(k.t);
                            string d = Convert.ToString(k.w);
                            if (b == "9")
                            {
                                string supersug = c.Replace(a, d);
                                if (checkword(supersug, 0)) { string[] supersugg = { supersug }; return supersugg; }
                                else
                                {
                                    //string[] sug1 = { supersug }; since supersug is wrong word as per above check
                                    string[] sug1 = { };
                                    string[] suggest = getsuggestion(c.Replace(a, "s"));//s for suggestion
                                    sug1 = sug1.Concat(suggest).ToArray();
                                    for (int l = 0; l < sug1.Length; l++) { sug1[l] = sug1[l].Replace("s", d); } return sug1;
                                }
                            }
                            else { sug = sug.Concat(getsample(b, c, a, d)).ToArray(); }
                        }
                }
            }

            sug = sug.Concat(getsample("100", c, "", "்")).ToArray();//special logics for ்
            sug = sug.Concat(getsample("100", c, "", "ா")).ToArray();//special logics for ா
            sug = sug.Concat(getsample("100", c, "", "ி")).ToArray();//special logic ி
            sug = sug.Concat(getsample("100", c, "", "ை")).ToArray();//special logic ை
            sug = sug.Concat(getsample("101", c, "", "")).ToArray();//special logics for ர-ா 
            sug = sug.Concat(getsample("102", c, "", "1")).ToArray();
            sug = sug.Concat(getsample("102", c, "", "2")).ToArray();
            sug = sug.Concat(getsample("102", c, "", "3")).ToArray();
            return sug;

        }

#endregion

#region get third level suggestions for given word
        public string[] getsuggestion3(string c, dynamic supl, int n)
        {
            //instead of using suplist directly, we have used serialize and deserialize
            //int o = Convert.ToInt16(supl.Count);//since count may null

            string jObj = JsonConvert.SerializeObject(supl);
            int o = jObj.Length;

            string jsonString;

            if ((o < 3) && (n < 1))
            {
                dynamic supgword = JsonConvert.DeserializeObject("{\"க\":[{\"t\":\"5\",\"w\":\"க்க\"}],\"க்க\":[{\"t\":\"0\",\"w\":\"க\"}],\"ச\":[{\"t\":\"5\",\"w\":\"ச்ச\"}],\"ச்ச\":[{\"t\":\"0\",\"w\":\"ச\"}],\"த\":[{\"t\":\"5\",\"w\":\"த்த\"}],\"த்த\":[{\"t\":\"0\",\"w\":\"த\"}],\"ப\":[{\"t\":\"5\",\"w\":\"ப்ப\"}],\"ப்ப\":[{\"t\":\"0\",\"w\":\"ப\"}],\"ர\":[{\"t\":\"0\",\"w\":\"ற\"}],\"ற\":[{\"t\":\"0\",\"w\":\"ர\"}],\"ல\":[{\"t\":\"0\",\"w\":\"ள\"},{\"t\":\"0\",\"w\":\"ழ\"}],\"ள\":[{\"t\":\"0\",\"w\":\"ல\"},{\"t\":\"0\",\"w\":\"ழ\"}],\"ழ\":[{\"t\":\"0\",\"w\":\"ல\"},{\"t\":\"0\",\"w\":\"ள\"}],\"ந\":[{\"t\":\"0\",\"w\":\"ன\"},{\"t\":\"0\",\"w\":\"ண\"}],\"ன\":[{\"t\":\"0\",\"w\":\"ந\"},{\"t\":\"0\",\"w\":\"ண\"}],\"ண\":[{\"t\":\"0\",\"w\":\"ன\"},{\"t\":\"0\",\"w\":\"ந\"}]}");
                jsonString = "";
                foreach (dynamic j in supgword)
                {//j gives every miswords
                    String a = Convert.ToString(j.Name);
                    if (c.Contains(a)) jsonString = jsonString + ",\"" + a + "\":" + JsonConvert.SerializeObject(supgword[a]);//suplist[j]=supgword[j];
                }
                jsonString = "{" + jsonString.Substring(1) + "}";//placing comma at begginging
            }
            else { jsonString = JsonConvert.SerializeObject(supl); }
            dynamic suplist = JsonConvert.DeserializeObject(jsonString);
            string[] sug2 = { };

            foreach (dynamic j in suplist)//list of required super suggestion list
            {
                string[] c2 = { };
                string a = Convert.ToString(j.Name);
                foreach (dynamic k in suplist[a])
                {//k is array of suggestions
                    string b = Convert.ToString(k.t);
                    string d = Convert.ToString(k.w);
                    string p = "p" + Convert.ToString(n);//To identify which loop, using p identifier and nth value
                    string[] getsamplec2 = getsample(b, c, a, p);
                    c2 = c2.Concat(getsamplec2).ToArray();
                    dynamic suplist2 = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(suplist));//to avoid original json change
                    suplist2.Remove(a);
                    sug2 = sug2.Concat(c2).ToArray();

                    string suplist2Obj = JsonConvert.SerializeObject(suplist2);
                    if (suplist2Obj.Length > 2) for (int l = 0; l < c2.Length; l++)
                        {
                            string[] getsuggestion2sug2 = getsuggestion3(c2[l], suplist2, n + 1);
                            sug2 = sug2.Concat(getsuggestion2sug2).ToArray();
                        }
                    for (int l = 0; l < sug2.Length; l++) { sug2[l] = sug2[l].Replace(p, d); }
                }
            }
            return sug2;
        }

#endregion

#region get second level suggestion for given word
        public string[] getsuggestion2(string word)
        {
            string[] sugword = new string[7] { "க்க,க", "ச்ச,ச", "த்த,த", "ப்ப,ப", "ற,ர", "ல,ள,ழ", "ந,ன,ண" };
            string[] sug = { };//[word.Substring(0,1)];
            //word=word.substr(1);//since first letter & last is not in this category
            int limit = word.Length;
            for (int h = 0; h < limit; h++)
            {
                string[] sug1 = { };
                bool flag = false;
                for (int i = 0; i < sugword.Length; i++)
                {
                    string[] poss = sugword[i].Split(',');
                    if (flag == false) for (int j = 0; j < poss.Length; j++)
                        {
                            if (flag == false) if (word.Length >= poss[j].Length)
                                    if (word.Substring(0, poss[j].Length) == poss[j])
                                    {
                                        word = word.Substring(poss[j].Length); sug1 = sug1.Concat(combination(sug, poss)).ToArray();
                                        flag = true; break;
                                    }
                        }
                }
                if (sug1.Length < 1) { if (word.Length > 0) { sug = combination(sug, new[] { word.Substring(0, 1) }); word = word.Substring(1); } }
                else
                { //sug1.CopyTo(sug, 0);//
                    Array.Resize(ref sug, sug1.Length);
                    Array.Copy(sug1, sug, sug1.Length);
                    if (sug.Length > 1000) return new string[0];//return empty arry to save error
                }
            }
            //sug=combination(sug,new[] {word.Substring(0,1)});
            return sug;
        }
        public string[] combination(string[] word, string[] sug)
        {
            if (word.Length == 0) return sug;
            string[] sug1 = { };
            foreach (string i in sug) foreach (string j in word) { sug1 = sug1.Concat(new[] { j + i }).ToArray(); }
            return sug1;
        }
        public string[] getsample(string code, string word, string fstr, string tstr)
        {
            //List<string> sample=new List<string>();
            string[] sample = { };
            //match will return கள since j is க[^ா-்]. capture return க. c1 will return match index or null
            switch (code)
            {

                case "0":
                    sample = sample.Concat(new[] { word.Replace(fstr, tstr) }).ToArray();
                    MatchCollection matches = Regex.Matches(word, fstr, RegexOptions.IgnoreCase); // Here we check the Match instance.
                    foreach (Match mat in matches)
                    {
                        GroupCollection groups = mat.Groups;
                        string c1 = mat.Groups[0].Value;
                        int count = mat.Groups[0].Index;
                        sample = sample.Concat(new[] { word.Substring(0, count) + tstr + word.Substring(count + fstr.Length) }).ToArray();
                    }
                    break;

                case "1"://should not be starting letter.should be end with perfect mei
                    //ச->ச்
                    MatchCollection cmatch1 = Regex.Matches(word, fstr + "([ா-்]|)", RegexOptions.IgnoreCase); // Here we check the Match instance.
                    if (cmatch1.Count > 0)
                    {
                        int incre = 0;//to calculate character different between fstr & tstr
                        string a = word;
                        foreach (Match mat in cmatch1)
                        {
                            GroupCollection groups = mat.Groups;
                            string c1 = mat.Groups[0].Value; string c2 = mat.Groups[1].Value;
                            int count = mat.Groups[0].Index;
                            if (ismat(c2, count))
                            {
                                a = a.Substring(0, count + incre) + tstr + a.Substring(count + c1.Length + incre);//all replace
                                incre += tstr.Length - c1.Length;
                                sample = sample.Concat(new[] { word.Substring(0, count) + tstr + word.Substring(count + c1.Length) }).ToArray();
                            }//eachone replace
                        }
                        sample = sample.Concat(new[] { a }).ToArray();
                    }
                    break;

                case "2"://should not be starting letter
                    MatchCollection cmatch2 = Regex.Matches(word, fstr, RegexOptions.IgnoreCase); // Here we check the Match instance.
                    if (cmatch2.Count > 0)
                    {
                        int incre = 0;//to calculate character different between fstr & tstr
                        string a = word;
                        foreach (Match mat in cmatch2)
                        {
                            GroupCollection groups = mat.Groups;
                            string c1 = mat.Groups[0].Value;//same as fstr
                            int count = mat.Groups[0].Index;
                            if (count > 0)
                            {
                                a = a.Substring(0, count + incre) + tstr + a.Substring(count + c1.Length + incre);//all replace
                                incre += tstr.Length - c1.Length;
                                sample = sample.Concat(new[] { word.Substring(0, count) + tstr + word.Substring(count + c1.Length) }).ToArray();
                            }//eachone replace
                        }
                        sample = sample.Concat(new[] { a }).ToArray();
                    }
                    break;

                case "3"://should only be last letter
                    /* Match mcase3 = Regex.Match(word, fstr, RegexOptions.IgnoreCase); // Here we check the Match instance.
                     if (mcase3.Groups[0].Index == (word.Length - fstr.Length)) { sample = sample.Concat(new[] { word.Substring(0, word.Length - fstr.Length) + tstr }).ToArray(); }//when it match with first character
                     break;*/
                    string thala = word.Substring(0, word.Length - fstr.Length);
                    if (word == thala + fstr) { sample = sample.Concat(new[] { thala + tstr }).ToArray(); }
                    break;

                case "9":
                    //since it is dual check, handled before income.
                    break;

                case "4"://should only be first letter
                    Match mcase4 = Regex.Match(word, fstr, RegexOptions.IgnoreCase); // Here we check the Match instance.
                    if (mcase4.Groups[0].Index == 0) { sample = sample.Concat(new[] { tstr + word.Substring(fstr.Length) }).ToArray(); }//when it match with first character
                    break;
                case "5"://replace each one
                    MatchCollection cmatch5 = Regex.Matches(word, fstr, RegexOptions.IgnoreCase); // Here we check the Match instance.
                    if (cmatch5.Count > 0)
                    {
                        int incre = 0;//to calculate character different between fstr & tstr
                        foreach (Match mat in cmatch5)
                        {
                            GroupCollection groups = mat.Groups;
                            string c1 = mat.Groups[0].Value;//same as fstr
                            int count = mat.Groups[0].Index;
                            if ((count > 0) && (count < word.Length - 1))
                            {
                                incre += tstr.Length - c1.Length;
                                sample = sample.Concat(new[] { word.Substring(0, count) + tstr + word.Substring(count + c1.Length) }).ToArray();
                            }//eachone replace
                        }
                    }
                    break;


                case "101":
                    //நரர்கள் -> நார்கள்
                    MatchCollection matches0 = Regex.Matches(word, "[க-ஹ]ர([^ா-்]|)", RegexOptions.IgnoreCase); // Here we check the Match instance.
                    if (matches0.Count > 0)
                    {
                        string a = word;
                        foreach (Match mat in matches0)
                        {
                            GroupCollection groups = mat.Groups;
                            int count = mat.Groups[1].Index;// actual index of ர
                            a = a.Substring(0, count - 1) + "ா" + a.Substring(count);//all replace
                            sample = sample.Concat(new[] { word.Substring(0, count - 1) + "ா" + word.Substring(count) }).ToArray();
                        }
                        sample = sample.Concat(new[] { a }).ToArray();
                    }
                    break;
                case "102"://for repeated scequence
                    int diff = Convert.ToInt16(tstr);
                    for (var i = 0; i < word.Length - (diff * 2); i++)
                    {
                        if (word.Substring(i, diff) == word.Substring(i + diff, diff))
                        { sample = sample.Concat(new[] { word.Substring(0, i) + word.Substring(i + diff) }).ToArray(); }
                    }
                    break;

                case "100":
                    //ச->ச்
                    MatchCollection matches2 = Regex.Matches(word, "[க-ஹ]([ா-்]|)", RegexOptions.IgnoreCase); // Here we check the Match instance.
                    if (matches2.Count > 0)
                    {
                        int incre = 0;//to calculate character different between fstr & tstr
                        string a = word;
                        foreach (Match mat in matches2)
                        {
                            GroupCollection groups = mat.Groups;
                            string c1 = mat.Groups[0].Value;//return [க-ஹ]([ா-்]|)
                            string c2 = mat.Groups[1].Value;//return ([ா-்]|)
                            int count = mat.Groups[0].Index;
                            if (ismat(c2, count))
                            {
                                a = a.Substring(0, count + incre) + c1 + tstr + a.Substring(count + c1.Length + incre);//all replace
                                incre += tstr.Length - fstr.Length;
                                sample = sample.Concat(new[] { word.Substring(0, count) + c1 + tstr + word.Substring(count + c1.Length) }).ToArray();
                            }//each one replace
                        }
                        sample = sample.Concat(new[] { a }).ToArray();
                    }


                    //க்ச->கச் ottru post shifter 
                    MatchCollection matches4 = Regex.Matches(word, "[க-ஹ]" + tstr + "[க-ஹ]([ா-்]|)", RegexOptions.IgnoreCase); // Here we check the Match instance.
                    foreach (Match mat in matches4)
                    {
                        GroupCollection groups = mat.Groups;
                        string c1 = mat.Groups[0].Value;
                        string c2 = mat.Groups[1].Value;
                        int count = mat.Groups[0].Index;
                        if (ismat(c2, count))
                        {
                            string a = word.Substring(0, count) + c1.Substring(0, 1) + c1.Substring(2, 1) + tstr + word.Substring(count + c1.Length);
                            sample = sample.Concat(new[] { a }).ToArray();
                        }
                    }

                    //கச்->க்ச ottru pre shifter படிபப்டியாக
                    MatchCollection matches5 = Regex.Matches(word, "[க-ஹ]([ா-்]|)[க-ஹ]" + tstr, RegexOptions.IgnoreCase); // Here we check the Match instance.
                    foreach (Match mat in matches5)
                    {
                        GroupCollection groups = mat.Groups;
                        string c1 = mat.Groups[0].Value;
                        string c2 = mat.Groups[1].Value;
                        int count = mat.Groups[0].Index;
                        if (ismat(c2, count))
                        {
                            string a = word.Substring(0, count) + c1.Substring(0, 1) + tstr + c1.Substring(1, 1) + word.Substring(count + c1.Length);
                            sample = sample.Concat(new[] { a }).ToArray();
                        }
                    }

                    //ப்ட்ட->ப்டட் doubleottru post shifter
                    MatchCollection matches6 = Regex.Matches(word, "[க-ஹ]" + tstr + "[க-ஹ]" + tstr + "[க-ஹ]([ா-்]|)", RegexOptions.IgnoreCase); // Here we check the Match instance.
                    foreach (Match mat in matches6)
                    {
                        GroupCollection groups = mat.Groups;
                        string c1 = mat.Groups[0].Value;
                        string c2 = mat.Groups[1].Value;
                        int count = mat.Groups[0].Index;
                        if (ismat(c2, count))
                        {
                            string a = word.Substring(0, count) + c1.Substring(0, 3) + c1.Substring(4, 1) + tstr + word.Substring(count + c1.Length);
                            sample = sample.Concat(new[] { a }).ToArray();
                        }
                    }

                    //பட்ட்->ப்டட் doubleottru pre shifter-unused since pratically ottru pre shifter do this
                    //word.replace(RegExp("[க-ஹ]([ா-்]|)[க-ஹ]"+tstr+"[க-ஹ]"+tstr, "gi"), function (c1,c2,count,word) {if(ismat(c2,count)){var a =word.substr(0,count)+c1.substr(0,1)+tstr+c1.substr(1,1)+c1.substr(3,2)+word.substr(count+c1.length);sample.push(a);}return ;});
                    break;

            }
            return sample;
        }
        public bool ismat(string v1, int v2)
        {
            //v1 needs to be blank since reg needs no ா-் nearby
            //v2 needs to be integer and not first letter
            if ((v2 > 0) && (v1 == "")) { return true; }
            else { return false; }
        }
#endregion

#region concating the suggestion alltogether to the suggestion of the wrong word
        public dynamic addparinthu(dynamic parinthu, int i, string w)
        {//does it check value in string with comma
            if (((string)parinthu[i, 1]).IndexOf(w) < 0)//to avoid duplicate suggestion
            {//since unable to increase the array size
                if ((int)(parinthu[i, 0]) > 0) { parinthu[i, 0] = (int)(parinthu[i, 0]) + 1; parinthu[i, 1] = parinthu[i, 1].ToString() + "," + w; }
                else { parinthu[i, 0] = 1; parinthu[i, 1] = w; }
            }
            return parinthu;
        }
        private void byproduct(string varthai, string suggest) { }

        public Boolean checkword(string sol, int type)
        {
            //  return true;
            //output true or false
            /*
            type = 0 -> any code
            type = 1 -> only peyar
            type = 2 -> only venai
            type = 3 -> peyar or venai
            type = 4 -> non deri(Z,Adjective,adverb,conjuction,int)
            type = 5 -> perfect noun without viku
            type = 6 ->
            type = 7 -> special used for suggestion check to avoid extensive derivatives
            */
            int sugges = 0;//zero = it is not suggestion word
            if (type == 7)
            {
                sugges = 1;
            }
            for (int a = sol.Length; a > 0; a--)
            {//It cuts from end. end is best since small word has many derivation like ഊ
                //for(int a=1;a<=sol.length;a++){//it cuts from starting
                string paku = sol.Substring(0, a);
                string viku = sol.Substring(a);//,sol.length
                dynamic qcode = Oword[paku];

                //try{
                if (qcode != null) if (qcode.Count > 0) foreach (dynamic b in qcode)
                        {
                            //try{
                            if (b.s != null) return false;//to save time for foreign words and misspelled words
                            string code = ((string)b.t).Substring(0, 1);
                            string subcode = ((string)b.t).Substring(1);
                            //Logger.log("code:"+qcode[b].t +", word:"+paku);
                            switch (type)
                            {
                                case 0:
                                    break;

                                case 1:
                                    if (((peyar.IndexOf(code) == -1) || (subcode != "15")) && (speyar.IndexOf(code) == -1)) continue;
                                    break;

                                case 2:
                                    if ((venai.IndexOf(code) == -1) || (subcode != "15")) continue;
                                    break;

                                case 3:
                                    if ((venai.IndexOf(code) == -1) && (peyar.IndexOf(code) == -1)) continue;
                                    break;

                                case 4:
                                    if (nonderi.IndexOf(code) == -1) continue;
                                    break;

                                case 6:
                                    if (subcode != "15") continue;
                                    break;

                                case 5:
                                    if ((peyar.IndexOf(code) != -1) && (paku == sol) && (speyar.IndexOf(code) != -1) && (code != "M")) { return true; } else { return false; }

                                //break; since unable to reach
                            }

                            if (checkviku(paku, viku, "", code, subcode, sugges))
                            {
                                return true;
                            }
                            //if(splitviku(viku,code,subcode)){return true;}it should not go to splitviku to avoid chain word fusion for all cases
                            //}catch{continue;}

                        }//end of b
                //}catch{}
            }//end of a
            return false;
        }
#endregion

#region third level suffix alanysis
        public Boolean splitviku(string viku, string cod, string scod, int sugges)
        {
            //used in verb and nouns with subcode 15
            //input  கிறவர்கள், ஆ  
            //input   ர்கள், H

            //Logger.log(viku +", "+cod);
            //if(viku=="0")viku="";
            for (int c = 1; c < viku.Length; c++)//starts from 1, zero given as last priority
            {
                string subpaku = viku.Substring(0, c);
                string subviku = viku.Substring(c);
                if (checkviku("", subpaku, subviku, cod, scod, sugges)) { return true; }
            }
            if (checkviku("", "", viku, cod, scod, sugges)) { return true; }//similar effect of c=0 as it is needed to உலக-றிய
            return false;
        }
        public String codeuyir(string part)
        {//part -> ெழுதினார்
            //Logger.log(part+"------");
            //் is that okay?
            if (part.Length < 1) return part;//To avoid empty string
            char elu = char.Parse(part.Substring(0, 1));
            if (part.Length > 1)
            {
                foreach (dynamic d in auyir)
                { String c = Convert.ToString(d.Name); if (Convert.ToString(elu) == c) { return part.Replace(c, Convert.ToString(auyir[c])); } }
                if ((CharUnicodeInfo.GetNumericValue(elu) > 2965) && (CharUnicodeInfo.GetNumericValue(elu) < 2997))
                    //Logger.log("அ"+part);
                    return "அ" + part;
            }
            return part;
        }


#endregion

#region second level suffix analysis
        public Boolean checkviku(string p, string v, string sv, string c, string sc, int sugges)//paku,viku,subviku,code,subcode
        {
            //Logger.log(p+", "+v+", "+sv+", "+c+", "+sc);
            //input  "கொடு","க்கப்படுதல்","","த","15" ->looped again
            //input  "","க்கப்பட","ுதல்","த","15" ->looped again
            //input  "க்கப்பட","ுதல்","","இ","15"  ->passed
            //output    true   or false

            //நிகழ, ்ந்தமையால், , ர, 15
            //்ந்த, மையால், , ഉ, 15
            //

            //to adjust null part as zero
            //if(v==""){v="0";}//v needs to be blank in C#
            //if(sv==""){sv="0";}
            //don't adjust sv as 0
            //for வ family noun prefix starts
            if (c == "അ")
            {
                if (sv != "") return false;//this special should act as prefix, so no sv
                string secondword = p.Substring(p.Length - 1) + v;//p.Length
                string sw = "";

                if (secondword.Length > 2)
                {
                    sw = secondword; foreach (dynamic e in vauyir)
                    { String d = Convert.ToString(e.Name); if (sw.Substring(0, 2) == d) { sw = sw.Replace(d, Convert.ToString(vauyir[d])); break; } } if (sw.Substring(0, 1) == "வ") sw = "அ" + sw.Substring(1);
                }//,sw.Length-1
                if (checkword(sw, 1)) { return true; } else if (sw != secondword) { secondword = p.Substring(p.Length - 1) + v; if (checkword(secondword, 1))return true; }//check the noun word
                return false;
            }
            //for வ noun prefix end
            //Logger.log("Hi");

            //for ய family noun prefix starts
            if (c == "ആ")
            {
                if (sv != "") return false;
                string secondword = p.Substring(p.Length - 1) + v;
                string sw = "";
                if (secondword.Length > 2)
                {
                    sw = secondword; foreach (dynamic e in yauyir)
                    { String d = Convert.ToString(e.Name); if (sw.Substring(0, 2) == d) { sw = sw.Replace(d, Convert.ToString(yauyir[d])); break; } } if (sw.Substring(0, 1) == "ய") sw = "அ" + sw.Substring(1);
                }
                if (checkword(sw, 1)) { return true; } else if (sw != secondword) { secondword = p.Substring(p.Length - 1) + v; if (checkword(secondword, 1))return true; }//check the noun word
                return false;
            }
            //for ய family noun prefix end

            //for consonant family noun prefix
            if (c == "ഇ") { if (sv != "")return false; string secondword = p.Substring(p.Length - 1) + v; if (checkword(secondword, 1))return true; return false; }

            //for vowel family verb prefix starts
            if (c == "ഉ") { if (sv != "")return false; string secondword = codeuyir(v); if (checkword(secondword, 2))return true; return false; }
            //check verb word

            //for consonant family verb prefix starts
            if (c == "ഊ") { if (sv != "")return false; string secondword = p.Substring(p.Length - 1) + v; if (checkword(secondword, 2))return true; return false; }
            //check verb word

            //for noun & verb
            if (c == "എ") { if (sv != "")return false; string secondword = p.Substring(p.Length - 1) + v; if (checkword(secondword, 6))return true; return false; }

            //special code should not move further as Eword[ഊ] is not available
            //For nonderivatives Z family
            if ((v == "") && (nonderi.IndexOf(c) != -1)) { return true; }
            string blocks = "";
            switch (sc)
            {
                case "15":// plural + vowel + vinai + urubu + peyar ஆக்கப்பூர்வம்,அரசியல்பூர்வ,எழுத்துப்பூர்வமாக
                    blocks = "01234567"; break;
                case "25"://same as 15 but no derivatives
                    blocks = "0123456"; break;
                case "07"://vowel + vinai + urubu + peyar
                    blocks = "012345"; break;
                case "10"://except special and plural
                    blocks = "01235"; break;
                case "11"://except special 
                    blocks = "012356"; break;
                case "09"://vowel  adjective eg:உடல்ரீதி, கணினிரீதியிலான-> since no urubu ிலான is not available
                    blocks = "02"; break;
                case "06":// vowel  + vinai   (Adjective) பொது,முதன்முறை
                    blocks = "023"; break;
                case "05"://vowel + urubu + peyar
                    blocks = "013"; break;
                case "04"://vowel  (noun and adverb) eg:கணிப்புப்படி
                    blocks = "03"; break;
                case "03"://urubu + peyar
                    blocks = "13"; break;
                case "02"://special மனப்பூர்வமாக 
                    blocks = "2"; break;
                case "01"://special & urubu
                    blocks = "1"; break;
                case "16"://special 
                    blocks = "3"; break;
                case "17"://special
                    blocks = "0"; break;
                case "08"://Peyar speical extension அநவரத  venai present echam சுவைப்பட,நாள்பட 
                    blocks = "4"; break;
                case "18"://special  -> 08 also takes 4 segment
                    blocks = "4"; break;
                case "19"://special
                    blocks = "5"; break;
                case "20"://special
                    blocks = "6"; break;
                case "21"://special
                    blocks = "7"; break;
                case "24"://echam verb is using
                    blocks = "34"; break;
            }

            for (var d = 0; d < 8; d++)
            {
                if (blocks.IndexOf(Convert.ToString(d)) == -1) continue;//it will ignore the blocks which are not related
                // if (Eword[c][d][v] != null)
                {
                    //dynamic samp = Eword[c][d];
                    try
                    { if (derivative((string)Eword[c][d][v], v, sv, sugges)) return true; }
                    catch { }
                }
            }




            //C# need v as blank
            if (sv == "") if (v != "") if (deri.IndexOf(c) != -1)
                    {//to avoid adverb,adjective..
                        if (splitviku(v, c, sc, sugges)) return true;
                    }
            //
            return false;

        }
#endregion

#region check wheather derivatives are possible for given root words
        public Boolean derivative(string part, string v, string sv, int sugges)
        {
            //① for extensive derivatives
            //② for extensive derivatives excluded for suggestion
            if (part != null)
            {
                if ((part.IndexOf("①") != -1) || ((part.IndexOf("②") != -1) && (sugges == 0)))//for extensive derivative
                {
                    string kudu = part.Substring(part.Length - 3, 1);//part.Length - 2
                    string elai = part.Substring(part.Length - 2);//part.Length
                    if (checkviku(v, sv, "", kudu, elai, sugges) == true)
                        return true;
                    if (splitviku(sv, kudu, elai, sugges))
                        return true;//"க்கப்பட":"1இ" டுதலும் needs another split
                }
                else { if (sv == "")return true; } //to return sucess if last part is blank
            }
            return false;
        }

#endregion

#region validate and return true if given word is tamil
        public Boolean istamil(string aword)
        {
            Boolean tamil = false;
            foreach (char a in aword)
            {
                if ((a >= 2944) && (a <= 3071))
                { return true; }
            }
            return tamil;
        }
#endregion

    }

}
#endregion