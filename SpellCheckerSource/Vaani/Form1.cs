using mshtml; //IHTMLStyleElement
using Newtonsoft.Json;//jsonconvert
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;//streamreader
using System.Linq;
using System.Net;//WebClient()
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;//CharUnicodeInfo
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using VaaniEditor;


#region namespace for spellchecker search
namespace Vaani
{

    public partial class Vaani : Form
    {
        VaaniEditor.Vaani checker = new VaaniEditor.Vaani();

        string[] userOword;
        string[] usergword;
        string[] phsingle = "ன்g,ங்,a,அ,A,ஆ,i,இ,I,ஈ,u,உ,U,ஊ,e,எ,E,ஏ,o,ஒ,O,ஓ,k,க்,g,க்,z,ழ்,w,ந்,t,ட்,s,ச்,c,ச்,j,ஜ்,h,ஹ்,S,ஸ்,r,ர்,R,ற்,d,ட்,p,ப்,b,ப்,m,ம்,y,ய்,n,ன்,N,ண்,l,ல்,L,ள்,v,வ்,q,ஃ".Split(',');
        string[] nisingle = "a,அ,q,ஆ,s,இ,w,ஈ,d,உ,e,ஊ,g,எ,t,ஏ,r,ஐ,c,ஒ,x,ஓ,z,ஔ,F,ஃ,h,க,b,ங,E,ஜ,],ஞ,o,ட,p,ண,l,த,;,ந,i,ன,j,ப,k,ம,',ய,m,ர,u,ற,n,ல,y,ள,/,ழ,v,வ,U,ஶ,W,ஷ,Q,ஸ,R,ஹ,f,்,L,:,O,[,P,],M,/,L,௱,N,ௐ,T,க்ஷ,Y,ஸ்ரீ,A,௹,S,௺,D,௸,Z,௳,X,௴,C,௵,V,௶,B,௷".Split(',');
        //t,h,i,j replaced with pure tamil
        string[] phdouble = "ச்ர்இ,ஸ்ரீ,அஅ,ஆ,இஇ,ஈ,உஉ,ஊ,எஎ,ஏ,ஒஒ,ஓ,க்அ,க,கஅ,கா,க்ஆ,கா,க்இ,கி,கிஇ,கீ,க்ஈ,கீ,க்உ,கு,குஉ,கூ,க்ஊ,கூ,க்எ,கெ,கெஎ,கே,க்ஏ,கே,க்ஒ,கொ,கொஒ,கோ,க்ஓ,கோ,கஉ,கௌ,கஇ,கை,ழ்அ,ழ,ழஅ,ழா,ழ்ஆ,ழா,ழ்இ,ழி,ழிஇ,ழீ,ழ்ஈ,ழீ,ழ்உ,ழு,ழுஉ,ழூ,ழ்ஊ,ழூ,ழ்எ,ழெ,ழெஎ,ழே,ழ்ஏ,ழே,ழ்ஒ,ழொ,ழொஒ,ழோ,ழ்ஓ,ழோ,ழஉ,ழௌ,ழஇ,ழை,ழ்ஹ்,ழ்,ன்ஜ்,ஞ்,ஞ்அ,ஞ,ஞஅ,ஞா,ஞ்ஆ,ஞா,ஞ்இ,ஞி,ஞிஇ,ஞீ,ஞ்ஈ,ஞீ,ஞ்உ,ஞு,ஞுஉ,ஞூ,ஞ்ஊ,ஞூ,ஞ்எ,ஞெ,ஞெஎ,ஞே,ஞ்ஏ,ஞே,ஞ்ஒ,ஞொ,ஞொஒ,ஞோ,ஞ்ஓ,ஞோ,ஞஉ,ஞௌ,ஞஇ,ஞை,ந்அ,ந,நஅ,நா,ந்ஆ,நா,ந்இ,நி,நிஇ,நீ,ந்ஈ,நீ,ந்உ,நு,நுஉ,நூ,ந்ஊ,நூ,ந்எ,நெ,நெஎ,நே,ந்ஏ,நே,ந்ஒ,நொ,நொஒ,நோ,ந்ஓ,நோ,நஉ,நௌ,நஇ,நை,ன்ட்,ந்,ட்அ,ட,டஅ,டா,ட்ஆ,டா,ட்இ,டி,டிஇ,டீ,ட்ஈ,டீ,ட்உ,டு,டுஉ,டூ,ட்ஊ,டூ,ட்எ,டெ,டெஎ,டே,ட்ஏ,டே,ட்ஒ,டொ,டொஒ,டோ,ட்ஓ,டோ,டஉ,டௌ,டஇ,டை,ந்ஹ்,ந்த்,ட்ஹ்,த்,த்அ,த,தஅ,தா,த்ஆ,தா,த்இ,தி,திஇ,தீ,த்ஈ,தீ,த்உ,து,துஉ,தூ,த்ஊ,தூ,த்எ,தெ,தெஎ,தே,த்ஏ,தே,த்ஒ,தொ,தொஒ,தோ,த்ஓ,தோ,தஉ,தௌ,தஇ,தை,ட்ஹ்,த்,ச்அ,ச,சஅ,சா,ச்ஆ,சா,ச்இ,சி,சிஇ,சீ,ச்ஈ,சீ,ச்உ,சு,சுஉ,சூ,ச்ஊ,சூ,ச்எ,செ,செஎ,சே,ச்ஏ,சே,ச்ஒ,சொ,சொஒ,சோ,ச்ஓ,சோ,சஉ,சௌ,சஇ,சை,ச்ஹ்,ஷ்,ஷ்அ,ஷ,ஷஅ,ஷா,ஷ்ஆ,ஷா,ஷ்இ,ஷி,ஷிஇ,ஷீ,ஷ்ஈ,ஷீ,ஷ்உ,ஷு,ஷுஉ,ஷூ,ஷ்ஊ,ஷூ,ஷ்எ,ஷெ,ஷெஎ,ஷே,ஷ்ஏ,ஷே,ஷ்ஒ,ஷொ,ஷொஒ,ஷோ,ஷ்ஓ,ஷோ,ஷஉ,ஷௌ,ஷஇ,ஷை,ஜ்அ,ஜ,ஜஅ,ஜா,ஜ்ஆ,ஜா,ஜ்இ,ஜி,ஜிஇ,ஜீ,ஜ்ஈ,ஜீ,ஜ்உ,ஜு,ஜுஉ,ஜூ,ஜ்ஊ,ஜூ,ஜ்எ,ஜெ,ஜெஎ,ஜே,ஜ்ஏ,ஜே,ஜ்ஒ,ஜொ,ஜொஒ,ஜோ,ஜ்ஓ,ஜோ,ஜஉ,ஜௌ,ஜஇ,ஜை,ச்ஹ்்,ஷ்்,ஹ்அ,ஹ,ஹஅ,ஹா,ஹ்ஆ,ஹா,ஹ்இ,ஹி,ஹிஇ,ஹீ,ஹ்ஈ,ஹீ,ஹ்உ,ஹு,ஹுஉ,ஹூ,ஹ்ஊ,ஹூ,ஹ்எ,ஹெ,ஹெஎ,ஹே,ஹ்ஏ,ஹே,ஹ்ஒ,ஹொ,ஹொஒ,ஹோ,ஹ்ஓ,ஹோ,ஹஉ,ஹௌ,ஹஇ,ஹை,-ச்,ஸ்,ஸ்அ,ஸ,ஸஅ,ஸா,ஸ்ஆ,ஸா,ஸ்இ,ஸி,ஸிஇ,ஸீ,ஸ்ஈ,ஸீ,ஸ்உ,ஸு,ஸுஉ,ஸூ,ஸ்ஊ,ஸூ,ஸ்எ,ஸெ,ஸெஎ,ஸே,ஸ்ஏ,ஸே,ஸ்ஒ,ஸொ,ஸொஒ,ஸோ,ஸ்ஓ,ஸோ,ஸஉ,ஸௌ,ஸஇ,ஸை,ர்அ,ர,ரஅ,ரா,ர்ஆ,ரா,ர்இ,ரி,ரிஇ,ரீ,ர்ஈ,ரீ,ர்உ,ரு,ருஉ,ரூ,ர்ஊ,ரூ,ர்எ,ரெ,ரெஎ,ரே,ர்ஏ,ரே,ர்ஒ,ரொ,ரொஒ,ரோ,ர்ஓ,ரோ,ரஉ,ரௌ,ரஇ,ரை,ற்அ,ற,றஅ,றா,ற்ஆ,றா,ற்இ,றி,றிஇ,றீ,ற்ஈ,றீ,ற்உ,று,றுஉ,றூ,ற்ஊ,றூ,ற்எ,றெ,றெஎ,றே,ற்ஏ,றே,ற்ஒ,றொ,றொஒ,றோ,ற்ஓ,றோ,றஉ,றௌ,றஇ,றை,ப்அ,ப,பஅ,பா,ப்ஆ,பா,ப்இ,பி,பிஇ,பீ,ப்ஈ,பீ,ப்உ,பு,புஉ,பூ,ப்ஊ,பூ,ப்எ,பெ,பெஎ,பே,ப்ஏ,பே,ப்ஒ,பொ,பொஒ,போ,ப்ஓ,போ,பஉ,பௌ,பஇ,பை,ம்அ,ம,மஅ,மா,ம்ஆ,மா,ம்இ,மி,மிஇ,மீ,ம்ஈ,மீ,ம்உ,மு,முஉ,மூ,ம்ஊ,மூ,ம்எ,மெ,மெஎ,மே,ம்ஏ,மே,ம்ஒ,மொ,மொஒ,மோ,ம்ஓ,மோ,மஉ,மௌ,மஇ,மை,ய்அ,ய,யஅ,யா,ய்ஆ,யா,ய்இ,யி,யிஇ,யீ,ய்ஈ,யீ,ய்உ,யு,யுஉ,யூ,ய்ஊ,யூ,ய்எ,யெ,யெஎ,யே,ய்ஏ,யே,ய்ஒ,யொ,யொஒ,யோ,ய்ஓ,யோ,யஉ,யௌ,யஇ,யை,ன்அ,ன,னஅ,னா,ன்ஆ,னா,ன்இ,னி,னிஇ,னீ,ன்ஈ,னீ,ன்உ,னு,னுஉ,னூ,ன்ஊ,னூ,ன்எ,னெ,னெஎ,னே,ன்ஏ,னே,ன்ஒ,னொ,னொஒ,னோ,ன்ஓ,னோ,னஉ,னௌ,னஇ,னை,ண்அ,ண,ணஅ,ணா,ண்ஆ,ணா,ண்இ,ணி,ணிஇ,ணீ,ண்ஈ,ணீ,ண்உ,ணு,ணுஉ,ணூ,ண்ஊ,ணூ,ண்எ,ணெ,ணெஎ,ணே,ண்ஏ,ணே,ண்ஒ,ணொ,ணொஒ,ணோ,ண்ஓ,ணோ,ணஉ,ணௌ,ணஇ,ணை,ங்அ,ங,ஙஅ,ஙா,ங்ஆ,ஙா,ங்இ,ஙி,ஙிஇ,ஙீ,ங்ஈ,ஙீ,ங்உ,ஙு,ஙுஉ,ஙூ,ங்ஊ,ஙூ,ங்எ,ஙெ,ஙெஎ,ஙே,ங்ஏ,ஙே,ங்ஒ,ஙொ,ஙொஒ,ஙோ,ங்ஓ,ஙோ,ஙஉ,ஙௌ,ஙஇ,ஙை,ல்அ,ல,லஅ,லா,ல்ஆ,லா,ல்இ,லி,லிஇ,லீ,ல்ஈ,லீ,ல்உ,லு,லுஉ,லூ,ல்ஊ,லூ,ல்எ,லெ,லெஎ,லே,ல்ஏ,லே,ல்ஒ,லொ,லொஒ,லோ,ல்ஓ,லோ,லஉ,லௌ,லஇ,லை,ள்அ,ள,ளஅ,ளா,ள்ஆ,ளா,ள்இ,ளி,ளிஇ,ளீ,ள்ஈ,ளீ,ள்உ,ளு,ளுஉ,ளூ,ள்ஊ,ளூ,ள்எ,ளெ,ளெஎ,ளே,ள்ஏ,ளே,ள்ஒ,ளொ,ளொஒ,ளோ,ள்ஓ,ளோ,ளஉ,ளௌ,ளஇ,ளை,வ்அ,வ,வஅ,வா,வ்ஆ,வா,வ்இ,வி,விஇ,வீ,வ்ஈ,வீ,வ்உ,வு,வுஉ,வூ,வ்ஊ,வூ,வ்எ,வெ,வெஎ,வே,வ்ஏ,வே,வ்ஒ,வொ,வொஒ,வோ,வ்ஓ,வோ,வஉ,வௌ,வஇ,வை,ன்த்,ந்த்,அஇ,ஐ,அஉ,ஒள,ட்ஹி,தி,ன்ஜ்,ஞ்,ன்ட்,ந்".Split(',');
        string[] nidouble = "கஆ,கா,கஇ,கி,கஈ,கீ,கஉ,கு,கஊ,கூ,கஎ,கெ,கஏ,கே,கஐ,கை,கஒ,கொ,கஓ,கோ,கஔ,கௌ,ஙஆ,ஙா,ஙஇ,ஙி,ஙஈ,ஙீ,ஙஉ,ஙு,ஙஊ,ஙூ,ஙஎ,ஙெ,ஙஏ,ஙே,ஙஐ,ஙை,ஙஒ,ஙொ,ஙஓ,ஙோ,ஙஔ,ஙௌ,சஆ,சா,சஇ,சி,சஈ,சீ,சஉ,சு,சஊ,சூ,சஎ,செ,சஏ,சே,சஐ,சை,சஒ,சொ,சஓ,சோ,சஔ,சௌ,ஜஆ,ஜா,ஜஇ,ஜி,ஜஈ,ஜீ,ஜஉ,ஜு,ஜஊ,ஜூ,ஜஎ,ஜெ,ஜஏ,ஜே,ஜஐ,ஜை,ஜஒ,ஜொ,ஜஓ,ஜோ,ஜஔ,ஜௌ,ஞஆ,ஞா,ஞஇ,ஞி,ஞஈ,ஞீ,ஞஉ,ஞு,ஞஊ,ஞூ,ஞஎ,ஞெ,ஞஏ,ஞே,ஞஐ,ஞை,ஞஒ,ஞொ,ஞஓ,ஞோ,ஞஔ,ஞௌ,டஆ,டா,டஇ,டி,டஈ,டீ,டஉ,டு,டஊ,டூ,டஎ,டெ,டஏ,டே,டஐ,டை,டஒ,டொ,டஓ,டோ,டஔ,டௌ,ணஆ,ணா,ணஇ,ணி,ணஈ,ணீ,ணஉ,ணு,ணஊ,ணூ,ணஎ,ணெ,ணஏ,ணே,ணஐ,ணை,ணஒ,ணொ,ணஓ,ணோ,ணஔ,ணௌ,தஆ,தா,தஇ,தி,தஈ,தீ,தஉ,து,தஊ,தூ,தஎ,தெ,தஏ,தே,தஐ,தை,தஒ,தொ,தஓ,தோ,தஔ,தௌ,நஆ,நா,நஇ,நி,நஈ,நீ,நஉ,நு,நஊ,நூ,நஎ,நெ,நஏ,நே,நஐ,நை,நஒ,நொ,நஓ,நோ,நஔ,நௌ,னஆ,னா,னஇ,னி,னஈ,னீ,னஉ,னு,னஊ,னூ,னஎ,னெ,னஏ,னே,னஐ,னை,னஒ,னொ,னஓ,னோ,னஔ,னௌ,பஆ,பா,பஇ,பி,பஈ,பீ,பஉ,பு,பஊ,பூ,பஎ,பெ,பஏ,பே,பஐ,பை,பஒ,பொ,பஓ,போ,பஔ,பௌ,மஆ,மா,மஇ,மி,மஈ,மீ,மஉ,மு,மஊ,மூ,மஎ,மெ,மஏ,மே,மஐ,மை,மஒ,மொ,மஓ,மோ,மஔ,மௌ,யஆ,யா,யஇ,யி,யஈ,யீ,யஉ,யு,யஊ,யூ,யஎ,யெ,யஏ,யே,யஐ,யை,யஒ,யொ,யஓ,யோ,யஔ,யௌ,ரஆ,ரா,ரஇ,ரி,ரஈ,ரீ,ரஉ,ரு,ரஊ,ரூ,ரஎ,ரெ,ரஏ,ரே,ரஐ,ரை,ரஒ,ரொ,ரஓ,ரோ,ரஔ,ரௌ,றஆ,றா,றஇ,றி,றஈ,றீ,றஉ,று,றஊ,றூ,றஎ,றெ,றஏ,றே,றஐ,றை,றஒ,றொ,றஓ,றோ,றஔ,றௌ,லஆ,லா,லஇ,லி,லஈ,லீ,லஉ,லு,லஊ,லூ,லஎ,லெ,லஏ,லே,லஐ,லை,லஒ,லொ,லஓ,லோ,லஔ,லௌ,ளஆ,ளா,ளஇ,ளி,ளஈ,ளீ,ளஉ,ளு,ளஊ,ளூ,ளஎ,ளெ,ளஏ,ளே,ளஐ,ளை,ளஒ,ளொ,ளஓ,ளோ,ளஔ,ளௌ,ழஆ,ழா,ழஇ,ழி,ழஈ,ழீ,ழஉ,ழு,ழஊ,ழூ,ழஎ,ழெ,ழஏ,ழே,ழஐ,ழை,ழஒ,ழொ,ழஓ,ழோ,ழஔ,ழௌ,வஆ,வா,வஇ,வி,வஈ,வீ,வஉ,வு,வஊ,வூ,வஎ,வெ,வஏ,வே,வஐ,வை,வஒ,வொ,வஓ,வோ,வஔ,வௌ,ஶஆ,ஶா,ஶஇ,ஶி,ஶஈ,ஶீ,ஶஉ,ஶு,ஶஊ,ஶூ,ஶஎ,ஶெ,ஶஏ,ஶே,ஶஐ,ஶை,ஶஒ,ஶொ,ஶஓ,ஶோ,ஶஔ,ஶௌ,ஷஆ,ஷா,ஷஇ,ஷி,ஷஈ,ஷீ,ஷஉ,ஷு,ஷஊ,ஷூ,ஷஎ,ஷெ,ஷஏ,ஷே,ஷஐ,ஷை,ஷஒ,ஷொ,ஷஓ,ஷோ,ஷஔ,ஷௌ,ஸஆ,ஸா,ஸஇ,ஸி,ஸஈ,ஸீ,ஸஉ,ஸு,ஸஊ,ஸூ,ஸஎ,ஸெ,ஸஏ,ஸே,ஸஐ,ஸை,ஸஒ,ஸொ,ஸஓ,ஸோ,ஸஔ,ஸௌ,ஹஆ,ஹா,ஹஇ,ஹி,ஹஈ,ஹீ,ஹஉ,ஹு,ஹஊ,ஹூ,ஹஎ,ஹெ,ஹஏ,ஹே,ஹஐ,ஹை,ஹஒ,ஹொ,ஹஓ,ஹோ,ஹஔ,ஹௌ".Split(',');

        Boolean fsource = true;
        string keyboard = "phonetic";


        #region initializing the function
        public Vaani()
        {


            InitializeComponent();
            panel1.Visible = false;
            panel2.Visible = false;

            panel3.Visible = false;
             panel4.Visible = true;
             customize.Visible = true;
             saveAsToolStripMenuItem.Visible = true;
             openToolStripMenuItem.Visible = true;
             பழதரததToolStripMenuItem.Visible = true;
             அகரதToolStripMenuItem.Visible = false;
              home.Visible = true;
             this.Text = "தமிழிணைய பிழைதிருத்தி";

             webBrowser1.DocumentText = "<html><body></body></html>";
            panel4.Size = new Size(this.ClientSize.Width + 1, this.ClientSize.Height); // .Size;
            panel3.Size = new Size(this.ClientSize.Width + 1, this.ClientSize.Height); //.Size;
            

            tBox.Text = "பொருமையாக இறுங்கள். இன்த திருத்தி உருவாகி வருகிரது";
            StreamReader r = new StreamReader(@"koppu\User.Txt");
            string txt = r.ReadToEnd();
            txt = txt.Replace("\r\n", "\n");//textbox
            string[] user = txt.Split('\n');
            r.Close();
            r.Dispose();
            userOword = user[0].Split(',');
            usergword = user[1].Split(',');
            keyboard = user[2];
            if (keyboard == "phonetic") { phonetic.Checked = true; tamil99.Checked = false; }
            else { tamil99.Checked = true; phonetic.Checked = false; }

        }
#endregion

        #region give suggestion when textbox has content
        private void getSuggestion(object sender, EventArgs e)
        {

            string word = rBox.SelectedText.Trim();
            if (word.Length < 1) return;

            sBox.Items.Clear();
            sBox.Enabled = true;
            //try            {
            string sug = null;
            if (!String.IsNullOrEmpty(hBox.Text))
            {

                dynamic sugword = JsonConvert.DeserializeObject(hBox.Text);
                sug = Convert.ToString(sugword[word]);
            }
            if ((sug != "wrong") && (!String.IsNullOrEmpty(sug)))
            {
                string[] sugg = sug.Split(',');
                foreach (string item in sugg) { sBox.Items.Add(item); }

                panel.Visible = true;
            }
            else if (sug == "wrong")
            {
                sBox.Enabled = false;
                panel.Visible = true;
            }
            else
            { panel.Visible = false; }
            //}           catch(){}

        }
        #endregion

        #region the function will be called when selected text get changed
        private void sBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            string word = rBox.SelectedText.Trim();

            /*rBox.SelectedText.Replace(word,sBox.SelectedItem.ToString());
            rBox.SelectionStart = 0;
            int len = rBox.SelectionLength;
            rBox.Text = rBox.Text.Remove(0, len);
            */
            if ((!string.IsNullOrEmpty(word)) && (sBox.SelectedIndex > -1))
            {
                rBox.SelectionColor = Color.Black;
                rBox.SelectedText = rBox.SelectedText.Replace(word, sBox.SelectedItem.ToString());
                panel.Visible = false;
                //rBox.Text = rBox.Text.Replace(word, sBox.SelectedItem.ToString()); 
            }


        }
        #endregion

        #region when button1 clicked for verification, the function will be called
        private void button1_Click(object sender, EventArgs e)
        {

            if (rBox.Text.Length > 0)//action for after correction
            {
                tBox.Text = rBox.Text.Replace("\n", "\r\n");// newline format
                rBox.Text = "";
                button1.Text = "சோதி";
                rBox.Visible = false;
                tBox.Visible = true;
            }
            else if (tBox.Text.Length > 0)//action for before correction
            {
                panel.Visible = false;
                hBox.Text = null;
                string aword = tBox.Text;
                string sword = "";
                string tword = "";
                Boolean tamil = true;
                foreach (char a in aword)
                {
                    if ((a >= 33) && (a <= 48)) { tword += "|"; sword += "|"; }
                    if ((a >= 2944) && (a <= 3071))  // if ((CharUnicodeInfo.GetNumericValue(a) >= 2944) && (CharUnicodeInfo.GetNumericValue(a) <= 3071))
                    { if (tamil) { tword += a; sword += a; } else { tword += "|" + a; sword += "|" + a; tamil = true; } }
                    else
                    { if (!tamil) { sword += a; } else { tword += "|"; sword += "|" + a; tamil = false; } }
                }

                string[] iword = sword.Split('|'); //{ "என்த","d sdfs ", "சத்துக்", " sasa as","கள்" };
                string[] tamword = tword.Split('|'); //{ "என்த","", "சத்துக்", "","கள்" };
                //VaaniEditor.Vaani checker = new VaaniEditor.Vaani();
                dynamic[,] rword = checker.gpathil11(tamword, true, "exe");
                tamword = tword.Split('|');// assigned again since sandhi removed on tamword after gpathil10
                button1.Text = "சம்மதம்";
                tBox.Visible = false;
                rBox.Visible = true;

                rBox.Clear();
                rBox.Text = tBox.Text;//copying whole content and then highlighting it later


                List<string> map1 = new List<String>();
                List<string> map2 = new List<String>();
                rBox.Font = new Font(rBox.SelectionFont, FontStyle.Regular);
                rBox.ForeColor = Color.Black;//default
                for (int i = 0; i < rword.GetLength(0); i++)
                {
                    if (rword[i, 1].ToString() == "wrong")
                    {
                        if (!map1.Contains(tamword[i]))
                        {
                            map1.Add(tamword[i]); map2.Add(rword[i, 1].ToString());
                            int Index = 0;
                            int limit = rBox.Text.LastIndexOf(iword[i]);
                            while (Index <= limit)//for multiple repeats
                            {
                                rBox.Find(iword[i], Index, rBox.TextLength, RichTextBoxFinds.WholeWord);
                                rBox.SelectionColor = Color.Black;
                                rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Underline);
                                Index = rBox.Text.IndexOf(iword[i], Index) + 1;
                            }
                        }
                    }

                    else if ((rword[i, 1].ToString() != "correct") && (rword[i, 1].ToString() != "wrong"))
                    {
                        if ((!String.IsNullOrEmpty(tamword[i])) && (!map1.Contains(tamword[i])))
                        {
                            map1.Add(tamword[i]); map2.Add(rword[i, 1].ToString());
                            int Index = 0;
                            int limit = rBox.Text.LastIndexOf(iword[i]);
                            while (Index <= limit)//for multiple repeats
                            {
                                rBox.Find(iword[i], Index, rBox.TextLength, RichTextBoxFinds.WholeWord);
                                rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Regular);
                                if ((rword[i, 1].ToString().IndexOf(",") == -1) && (autoc.Checked))
                                {
                                    rBox.SelectionColor = Color.Green;
                                    rBox.SelectedText = rBox.SelectedText.Replace(iword[i].ToString(), rword[i, 1].ToString());
                                    Index = rBox.Text.IndexOf(rword[i, 1].ToString(), Index) + 1;
                                    if (Index == 0) { Index = limit + 1; }
                                }
                                else
                                {
                                    rBox.SelectionColor = Color.Red;
                                    Index = rBox.Text.IndexOf(iword[i], Index) + 1;
                                }
                            }
                        }
                        //rBox.SelectionColor = Color.Black;
                        //rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Regular);
                    }
                }
                string result = "";
                for (int i = 0; i < map1.Count; i++)
                { result = result + "\"" + map1[i] + "\":\"" + map2[i] + "\","; }
                if (result.Length > 2) { hBox.Text = "{" + result.Substring(0, result.Length - 1) + "}"; }

            }
        }

        #endregion

        #region when contatc button is clicked below message will be showed
        private void contact_Click(object sender, EventArgs e)
        {
            //using (new VaaniBox(this))
            {
                //MessageBox.Show("வாணி சொற்பிழை திருத்தி சார்ந்த தொடர்புகளுக்கு neechalkaran@gmail.com என்ற முகவரியைத் தொடர்பு  கொள்ளலாம்", "தொடர்புக்கு" );
                MessageBox.Show("neechalkaran@gmail.com என்ற முகவரியைத் தொடர்பு  கொள்ளலாம்", "தொடர்புக்கு");
            }

        }
        #endregion
        private void customize_Click(object sender, EventArgs e) { }
        

        #region when about us button clicked, it shows the developer detail
        private void aboutus_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("இணைய சொற்பிழை திருத்தி" + Environment.NewLine + Environment.NewLine + "தமிழ் இணையக்கல்விக் கழகத்தின் நிதி உதவியுடன் http://vaani.neechalkaran.com இணையதளம் சார்பாகத் தயாரிக்கப்பட்டு வெளியிடப்படுகிறது. தமிழ் மொழியின் ஆக்கப்பூர்வமான பயன்பாட்டிற்கு இம்மென்பொருளை யாருடனும் பகிர்ந்துகொள்ளலாம், பயன்படுத்தலாம்.", "சுயவிபரம்");
            MessageBox.Show("இணைய அகராதி" + Environment.NewLine + Environment.NewLine + "தமிழ் இணையக்கல்விக் கழகத்தின் நிதி உதவியுடன் http://vaani.neechalkaran.com இணையதளம் சார்பாகத் தயாரிக்கப்பட்டு வெளியிடப்படுகிறது. தமிழ் மொழியின் ஆக்கப்பூர்வமான பயன்பாட்டிற்கு இம்மென்பொருளை யாருடனும் பகிர்ந்துகொள்ளலாம், பயன்படுத்தலாம்.", "சுயவிபரம்");
        }
        #endregion

        #region when help button is clicked, then below pdf will be opened
        private void help_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("தமிழ் வாக்கியங்களை பெட்டியில் இட்டு சோதி பொத்தானை அழுத்தவும். சில நொடிகளில் ஆய்வு செய்து திருத்திய வாக்கியங்கள் புதிய பெட்டியொன்றில் காட்டப்படும். பிழையான அல்லது தெரியாத சொற்கள் அடிக்கோடிட்டும், பரிந்துரைகளுள்ள சொற்கள் செந்நிறத்திலும் காட்சிதரும். செந்நிறச் சொற்கள் மீது இரட்டைச் சொடுக்கு செய்தால் வரும் பரிந்துரைச் சொற்களைத் தேர்வுசெய்து திருத்திக் கொள்ளலாம். விரும்பிய திருத்தங்கள் செய்தபிறகு சம்மதம் பொத்தானை அழுத்தி மூலவாக்கியங்களைத் திருத்தலாம்.", "உதவி");
            System.Diagnostics.Process.Start(@"koppu\User Manuals.pdf");
        }
        #endregion

        #region to add new words 
        private void addword_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader r = new StreamReader(@"koppu\User.Txt");
                string txt = r.ReadToEnd();
                string[] user = txt.Split('\n');
                string[] words = user[0].Split(',');
                listBox1.Items.Clear();
                foreach (string a in words)
                { listBox1.Items.Add(a); }
                r.Close();
                r.Dispose();
            }
            catch { MessageBox.Show("மன்னிக்கவும்.பயனர் தரவுகளில் வழுவுள்ளது"); return; }
            textBox1.Text = "";
            textBox2.Text = "";
            panel1.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            panel2.BringToFront();
            label3.Text = "பயனர் சொற்பட்டியல்";
            textBox2.Visible = false;
        }
        #endregion

        #region to add new suggestion
        private void addsuggestion_Click(object sender, EventArgs e)
        {
            try
            {
                StreamReader r = new StreamReader(@"koppu\User.Txt");
                string txt = r.ReadToEnd();

                string[] user = txt.Split('\n');
                string[] words = user[1].Split(',');
                listBox1.Items.Clear();
                foreach (string a in words)
                { listBox1.Items.Add(a); }
                r.Close();
                r.Dispose();
            }
            catch { MessageBox.Show("மன்னிக்கவும்.பயனர் தரவுகளில் வழுவுள்ளது"); return; }
            panel1.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            textBox1.Text = "";
            textBox2.Text = "";
            label3.Text = "பயனர் பரிந்துரைப்பட்டியல்";
            textBox2.Visible = true;

        }
        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            //should be Tamil
            //should not be in database and here
            //need duplicate check
            string tword = textBox1.Text + textBox2.Text;
            if ((tword.IndexOf(',') > -1) || (tword.IndexOf('|') > -1)) { MessageBox.Show("சிறப்புக் குறியீடுகளைத் தவிர்க்கவும்"); return; }
            string word = textBox1.Text;
            if (textBox2.Visible) { word += "|" + textBox2.Text; }
            if (word == "") { return; }
            try
            {

                StreamReader r = new StreamReader(@"koppu\User.txt");
                string txt = r.ReadToEnd();
                r.Close();
                txt = txt.Replace("\r\n", "\n");
                string[] user = txt.Split('\n');
                if (textBox2.Visible) { user[1] = user[1] + "," + word; }
                else { user[0] = user[0] + "," + word; }
                string matter = String.Join("\r\n", user);

                StreamWriter file = new StreamWriter(@"koppu\User.txt");
                file.Write(matter);
                file.Close();
                file.Dispose();
                listBox1.Items.Add(word);

            }
            catch { MessageBox.Show("மன்னிக்கவும்.பயனர் தரவுகளில் வழுவுள்ளது"); return; }


        }

        private void home_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = true;
            richTextBox1.Clear();
        }
        private void dic_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            webBrowser1.Visible = true;
            panel4.Visible = false;
        }

        #region when textbox content changed
        //private void tBox_TextChanged(object sender, KeyEventArgs e)
        private void tBox_TextChanged(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            int co = (int)e.KeyChar;
            //if (((co >= 65) && (co <= 90)) || ((co >= 97) && (co <= 122)))
            if ((co >= 32) && (co <= 126))
            {
                e.Handled = true; //on keypress event, it skip keypress added to textbox
                //cache is not needed
                tBox.SelectedText = "";//to impliment replace event
                transliterate(tBox, e.KeyChar);
            }
        }
#endregion


        #region capture the user keypress
        private void Usug_changed(object sender, KeyPressEventArgs e)
        {//user suggestion textbox
            int co = (int)e.KeyChar;

            if (co != 8)
            {
                e.Handled = true;
                transliterate(Usug, e.KeyChar);
            }
        }
        #endregion

        #region when textbox1 get changed below function will be called and text will be transliterated
        private void textBox1_TextChanged(object sender, KeyPressEventArgs e)
        {
            int co = (int)e.KeyChar;
            if (co != 8)
            {
                e.Handled = true;
                
                transliterate(textBox1, e.KeyChar);
            }
        }
        #endregion

        #region when textbox2 get changed below function will be called and text will be transliterated
        private void textBox2_TextChanged(object sender, KeyPressEventArgs e)
        {
            int co = (int)e.KeyChar;
            if (co != 8)
            {
                e.Handled = true;
                transliterate(textBox2, e.KeyChar);
            }
        }
        #endregion

        #region when loading happens, it clears the past content
        private void loading()
        {

            // webBrowser1.Navigate("about:blank");  
            //    webBrowser1.Document.Body.InnerText = "<center>தேடல் ...</center>";
            webBrowser1.DocumentText = "<html><body><center>தேடல் ...</center></body></html>";
            
            //webBrowser1.Refresh();
        }
        #endregion

        #region searching for words initialization
        private void searching()
        {

            string tword = textBox3.Text;

            string result = searchme(tword, true);
            try
            {
                if (result == "Give Tamil word") {
                webBrowser1.DocumentText = ""; 
                MessageBox.Show("Please Give Tamil word");
                
                }
                else
                {
                    webBrowser1.DocumentText = result;
                }
                //webBrowser1.Document.Body.InnerText = result;

                //webBrowser1.DocumentText = webBrowser1.DocumentText;
                webBrowser1.Refresh();
                //  setbrowsersize(); 
            }
            catch { }
             Cursor.Current = Cursors.Default;

             //this.ActiveControl = textBox3;
            return;
        }
        #endregion

        #region when textbox3 get changed below function will be called and transliteration will happen
        private void textBox3_TextChanged(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            int co = (int)e.KeyChar;
            if (co == 13)
            {

                if (textBox3.Text == "") return;
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    ThreadStart childref = new ThreadStart(loading);
                    ThreadStart childref2 = new ThreadStart(searching);
                    //Console.WriteLine("In Main: Creating the Child thread");
                    Thread childThread = new Thread(childref);
                    childThread.Start();
                    Thread childThread2 = new Thread(childref2);
                    childThread2.Start();
                }
                catch { }
                //  Cursor.Current = Cursors.Default;
            }
           // else if (((co >= 65) && (co <= 90)) || ((co >= 97) && (co <= 122)))//(co!=8)
            else if((co >= 32) && (co <= 126))
            {

                if (checkBox1.Checked == true)
                {
                    fsource = false;
                    e.Handled = true;
                    transliterate(textBox3, e.KeyChar);
                    //Task.Run(() => fillsource());
                    fsource = true;
                }
                fillsource();
            }
            //fillsource();
        }
        #endregion

        #region transliteration on richtextbox
        private void transliterateRT(RichTextBox Box, char input)
        {
            //Box.Select()

            int pos = Box.SelectionStart;// tBox.Text.Length;
            string prefix = "";
            string fix = "";
            string suffix = "";
            //if (pos < 1){ return;}
            if (pos < 3)
            {
                prefix = "";
                fix = Box.Text.Substring(0, pos);
                suffix = Box.Text.Substring(pos);
                fix = translate(fix, input);
                Box.Text = prefix + fix + suffix;
            }
            else
            {
                prefix = Box.Text.Substring(0, pos - 2);
                fix = Box.Text.Substring(pos - 2, 2);
                suffix = Box.Text.Substring(pos);
                Box.Select(pos - 2, 2);
                string temp = fix;
                fix = translate(fix, input);
                //        Regex RX = new Regex(GetRtfUnicodeEscapedString(temp));
                //MatchCollection matches = RX.Matches(Box.SelectedRtf);
                //string colo = Box.SelectedRtf;
                //string t1 = GetRtfUnicodeEscapedString(temp);
                //string t2 = GetRtfUnicodeEscapedString(fix);
                //t1 = t1.Replace(" ", "\\cf0\\ulnone ");
                //t2 = t2.Replace(" ", "\\cf0\\ulnone ");
                //colo = Box.SelectedRtf.Replace(t1, t2);         
                //  Box.Rtf = colo;
                if (temp.Substring(temp.Length - 1) == " ") { Box.Select(pos - 1, 1); temp = " "; fix = fix.Substring(1); prefix = Box.Text.Substring(0, pos - 1); }

                Box.SelectedText = Box.SelectedText.Replace(temp, fix);
            }
            //fix = translate(fix);
            //Box.Text = prefix + fix + suffix;
            //string colo = GetRtfUnicodeEscapedString(fix)+"\\cf0\\ulnone";

            //Box.Rtf = Box.Rtf.Replace("\\cf0\\ulnone\\cf0\\ulnone", "\\cf0\\ulnone");
            Box.Select(prefix.Length + fix.Length, 0);//To move cursor
            //   if (Box.Multiline == true) Box.ScrollToCaret();


        }
        #endregion

        #region  transliteration for textbox
        private void transliterate(TextBox Box, char input)
        {

            //Box.Select()
            int pos = Box.SelectionStart;// tBox.Text.Length;
            string prefix = "";
            string fix = "";
            string suffix = "";
            //if (pos < 1){ return;}
            if (pos < 3)
            {
                prefix = "";
                fix = Box.Text.Substring(0, pos);
                suffix = Box.Text.Substring(pos);
                fix = translate(fix, input);
                Box.Text = prefix + fix + suffix;
            }
            else
            {
                prefix = Box.Text.Substring(0, pos - 2);
                fix = Box.Text.Substring(pos - 2, 2);
                suffix = Box.Text.Substring(pos);

                string temp = fix;
                fix = translate(fix, input);
                Box.Select(pos - 2, 2);
                Box.SelectedText = Box.SelectedText.Replace(temp, fix);
            }
            //fix = translate(fix);
            //Box.Text = prefix + fix + suffix;
            Box.Select(prefix.Length + fix.Length, 0);//To move cursor
            if (Box.Multiline == true) Box.ScrollToCaret();

        }
        private string translate(string txt, char input)
        {// It transliterate only typed letter.
            /*string a = txt.Substring(0,txt.Length-1);
            string b = txt.Substring(txt.Length-1);//takes last character
            */

            string a = txt;            
            string b = Convert.ToString(input);
            string c = "";
            if (checkBox1.Checked != true) { return a + b; }
            if (checkBox2.Checked != true) { return a + b; }
            Boolean space = false;
            if (a.Length == 2) if (!ischar(Convert.ToChar(a.Substring(1)))) { space = true; }
            if (keyboard == "phonetic")
            {
                if (space == false) { c = a + b; } else { c = b; }//to avoid before letter change
                for (int i = 0; i < phsingle.Length - 1; i++)
                {
                    c = c.Replace(phsingle[i], phsingle[i + 1]);
                    i = i + 1;
                }
                if (space == true) { c = a + c; }
                for (int i = 0; i < phdouble.Length - 1; i++)
                {
                    c = c.Replace(phdouble[i], phdouble[i + 1]);
                    i = i + 1;
                }
            }
            else
            {
                for (int i = 0; i < nisingle.Length - 1; i++)
                {
                    b = b.Replace(nisingle[i], nisingle[i + 1]);
                    i = i + 1;
                }
                c = a + b;
                for (int i = 0; i < nidouble.Length - 1; i++)
                {
                    c = c.Replace(nidouble[i], nidouble[i + 1]);
                    i = i + 1;
                }

                //special mappings for Tamil99
                if (a.Length > 1) if ((Convert.ToChar(b) >= 2965) && (Convert.ToChar(b) <= 3001))
                    {
                        if ((a.Substring(a.Length - 1) == b) && (a.Substring(a.Length - 2, 1) != "்")) { c = a + "்" + b; }//க+க=க்க
                        if ((a.Substring(a.Length - 1) == "ங") && (b == "க") && (a.Substring(a.Length - 2, 1) != "்")) { c = a + "்" + b; }//ங+க=ங்க
                        if ((a.Substring(a.Length - 1) == "ஞ") && (b == "ச") && (a.Substring(a.Length - 2, 1) != "்")) { c = a + "்" + b; }
                        if ((a.Substring(a.Length - 1) == "ந") && (b == "த") && (a.Substring(a.Length - 2, 1) != "்")) { c = a + "்" + b; }
                        if ((a.Substring(a.Length - 1) == "ண") && (b == "ட") && (a.Substring(a.Length - 2, 1) != "்")) { c = a + "்" + b; }
                        if ((a.Substring(a.Length - 1) == "ம") && (b == "ப") && (a.Substring(a.Length - 2, 1) != "்")) { c = a + "்" + b; }
                        if ((a.Substring(a.Length - 1) == "ன") && (b == "ற") && (a.Substring(a.Length - 2, 1) != "்")) { c = a + "்" + b; }
                    }
            }
            return c;
        }
        #endregion

        private void rBox_change(object sender, EventArgs e)
        {//unused

        }


        #region set default values 
        private void clear_Click(object sender, EventArgs e)
        {
            tBox.Clear();
            sBox.Items.Clear();
            rBox.Clear();
            rBox.Visible = false;
            button1.Text = "சோதி";
            tBox.Visible = true;
            rBox.Visible = false;
            panel.Visible = false;
            Usug.Clear();
        }
        #endregion

        #region when replace menu is selected then below function will be called and new word will be replaced
        private void replace_Click(object sender, EventArgs e)
        {
            if (Usug.Text.Length < 1) return;
            string sug = Usug.Text.Trim();
            string word = rBox.SelectedText.Trim();
            if (!string.IsNullOrEmpty(word))
            {
                rBox.SelectionColor = Color.Black;
                rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Regular);
                rBox.SelectedText = rBox.SelectedText.Replace(word, sug);
                panel.Visible = false;
                Usug.Text = "";
                //rBox.Text = rBox.Text.Replace(word, sBox.SelectedItem.ToString()); 
            }

        }
        #endregion

        #region when button2 clicked, then below function will be called and new word will be replaced all
        private void button2_Click(object sender, EventArgs e)
        {
            if (Usug.Text.Length < 1) return;
            string sug = Usug.Text.Trim();
            string word = rBox.SelectedText.Trim();
            if (!string.IsNullOrEmpty(word))
            {
                int Index = 0;
                int limit = rBox.Text.LastIndexOf(word);
                while (Index <= limit)//for multiple repeats
                {
                    rBox.Find(word, Index, rBox.TextLength, RichTextBoxFinds.WholeWord);
                    rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Regular);
                    rBox.SelectionColor = Color.Black;
                    rBox.SelectedText = rBox.SelectedText.Replace(word, sug);
                    if (rBox.Text.IndexOf(word, Index) == -1) break;
                    Index = rBox.Text.IndexOf(word, Index);//no increment is needed 
                }

                panel.Visible = false;
                Usug.Text = "";
                //rBox.Text = rBox.Text.Replace(word, sBox.SelectedItem.ToString()); 
            }

        }
        #endregion

        #region  set tamil99 as keyboard layout
        private void tamil99_Click(object sender, EventArgs e)
        {
            setlang("tamil99");
            tamil99.Checked = true;
            phonetic.Checked = false;
            keyboard = "tamil99";
        }
        #endregion

        #region set phonetic as keyboard layout
        private void phonetic_Click(object sender, EventArgs e)
        {
            setlang("phonetic");
            tamil99.Checked = false;
            phonetic.Checked = true;
            keyboard = "phonetic";

        }
        #endregion

        #region get user setting
        private void setlang(string lan)
        {
            StreamReader r = new StreamReader(@"koppu\User.txt");
            string txt = r.ReadToEnd();
            r.Close();
            txt = txt.Replace("\r\n", "\n");
            string[] user = txt.Split('\n');
            user[2] = lan;
            string matter = String.Join("\r\n", user);
            StreamWriter file = new StreamWriter(@"koppu\User.txt");
            file.Write(matter);
            file.Close();
            file.Dispose();
        }

        #endregion

        #region when search button clicked, loading and searching function will be called
        private void searchdic_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (textBox3.Text == "") { MessageBox.Show("தேடலுக்குச் சொல்லில்லை");
            //this.ActiveControl = textBox3;
            this.Cursor = Cursors.Default;
            this.textBox3.Focus(); return;
            }
            ThreadStart childref = new ThreadStart(loading);
            ThreadStart childref2 = new ThreadStart(searching);
            //Console.WriteLine("In Main: Creating the Child thread");
            Thread childThread = new Thread(childref);
            childThread.Start();
            Thread childThread2 = new Thread(childref2);
            childThread2.Start();
            this.Cursor = Cursors.Default;
            this.textBox3.Select();
        }
        #endregion

        #region search function is initiated and assign corresponding file names
        private string searchme(string tword, bool vagai)
        {//full result 
            if (tword.Length < 0) { return ""; }
            string initial = tword.Substring(0, 1);
            String file = "";
            Regex tamil = new Regex(@"[ஂ-௺]");
            if (!tamil.IsMatch(tword)) { return "Give Tamil word"; }
            Regex aa = new Regex(@"[அஆ]");
            Regex ee = new Regex(@"[இஈஉஊ]");
            Regex oo = new Regex(@"[எஏஐஒஓஒள]");
            Regex ka = new Regex(@"[க]");
            Regex cha = new Regex(@"[ச]");
            Regex tha = new Regex(@"[த]");
            Regex na = new Regex(@"[ந]");
            Regex pa = new Regex(@"[ப]");
            Regex ma = new Regex(@"[ம]");
            Regex va = new Regex(@"[வ]");
            Regex other = new Regex(@"[^அஆஇஈஉஊஎஏஐஒஓஔகசதபமநவ]");

            if (aa.IsMatch(initial)) { file = "aa"; }
            else if (ee.IsMatch(initial)) { file = "ee"; }
            else if (oo.IsMatch(initial)) { file = "oo"; }
            else if (ka.IsMatch(initial)) { file = "ka"; }
            else if (cha.IsMatch(initial)) { file = "cha"; }
            else if (tha.IsMatch(initial)) { file = "tha"; }
            else if (na.IsMatch(initial)) { file = "na"; }
            else if (pa.IsMatch(initial)) { file = "pa"; }
            else if (ma.IsMatch(initial)) { file = "ma"; }
            else if (va.IsMatch(initial)) { file = "va"; }
            else if (other.IsMatch(initial)) { file = "other"; }
            else { file = "ஃ"; }
            //Debug.WriteLine(file);
            string data1 = "";
            if (vagai == false)
            {
                string title = VaaniEditor.Vaani.getDB(@"koppu\" + file + @"h.json");
                return title;
            }
            try
            {
                data1 = VaaniEditor.Vaani.getDB(@"koppu\" + file + @".json");

            }
            catch (FileNotFoundException ex)
            {
            }
            dynamic db = JsonConvert.DeserializeObject(data1);
            dynamic result = "";

            if (db[tword] == null)
            {
                //common test = new common();
                VaaniEditor.Vaani test = new VaaniEditor.Vaani();
                dynamic[] wordroot = test.checkroot(tword);

                if (db[wordroot[0]] == null)
                {

                    return "<center>இச்சொல் அகராதியில் இல்லை</center>";
                }
                else { result = wordroot[0] + wordroot[1] + "<br/>"; tword = wordroot[0]; }
            }
            string styles = "style='width:100%;padding:3px;display: block;text-decoration: none;background:#C2DEE6;border-left: 8px solid #00A3CC;-moz-border-radius: 9px 3px 9px 3px;-webkit-border-radius: 3px;-webkit-border-top-left-radius: 9px;-webkit-border-bottom-right-radius:9px;'";

            //idttam
            if (db[tword].g != "")
            {
                string content = "";
                string[] row = Convert.ToString(db[tword].g).Split('ￜ');
                foreach (string i in row)
                {
                    if (i != "")
                    {
                        string[] cell = i.Split('¦');
                        content += cell[0] + "<br/>";
                    }
                }
                result += "<a class='result-panel' " + styles + " target='_blank' href='http://www.tamilvu.org/slet/pmdictionary/ldttamtse.jsp?editor=" + Uri.EscapeUriString(tword) + "'>தமிழ் - தமிழ் அகரமுதலி </a><table><tr><td><ul>" + content + "</ul></td></tr></table><br/>";
            }
            //lexhome
            if (db[tword].h != "")
            {
                string content = "";
                string[] row = Convert.ToString(db[tword].h).Split('ￜ');
                foreach (string i in row)
                {
                    if (i != "")
                    {
                        string[] cell = i.Split('¦');
                        content += "<li>" + cell[0] + " " + cell[1] + "</li>";
                    }
                }
                result += "<a class='result-panel' " + styles + " target='_blank' href='http://www.tamilvu.org/slet/servlet/srchlxpg?editor=" + Uri.EscapeUriString(tword) + "'>தமிழ் லெக்சிகன் - Tamil Lexicon</a><table><tr><td><ul>" + content + "</ul></td></tr></table><br/>";
            }
            //fabricus
            if (db[tword].f != "")
            {
                string content = "";
                string page = "";
                string[] row = Convert.ToString(db[tword].f).Split('ￜ');
                foreach (string i in row)
                {
                    if (i != "")
                    {
                        string[] cell = i.Split('¦');
                        content += "<li>" + cell[0] + "</li>";
                        page = cell[1];
                    }
                }
                content = content.Replace("<p1>", "");
                content = content.Replace("<col ", "<spam ");
                result += "<a class='result-panel' " + styles + " target='_blank' href='http://dsalsrv02.uchicago.edu/cgi-bin/romadict.pl?table=fabricius&page=" + page + "'>பாப்ரிசிஸ் அகராதி - J.P. Fabricius Dictionary</a><table><tr><td><ul>" + content + "</ul></td></tr></table><br/>";
            }
            //kadirvelu
            if (db[tword].k != "")
            {
                string content = "";
                string page = "";
                string[] row = Convert.ToString(db[tword].k).Split('ￜ');
                foreach (string i in row)
                {
                    if (i != "")
                    {
                        string[] cell = i.Split('¦');
                        content += cell[0] + "<br/>";
                        page = cell[1];
                    }
                }
                result += "<a class='result-panel' " + styles + " target='_blank' href='http://dsalsrv02.uchicago.edu/cgi-bin/romadict.pl?table=kadirvelu&page=" + page + "'>கதிர்வேலு அகராதி -Na Kadirvelu Pillai Dictionary</a><table><tr><td>" + content + "</td></tr></table><br/>";
            }
            //mcalpin
            if (db[tword].m != "")
            {
                string content = "";
                string page = "";
                string[] row = Convert.ToString(db[tword].m).Split('ￜ');
                foreach (string i in row)
                {
                    if (i != "")
                    {
                        string[] cell = i.Split('¦');
                        content += "<li>" + cell[0] + "</li>";
                        page = cell[1];
                    }
                }
                result += "<a class='result-panel' " + styles + " target='_blank' href='http://dsalsrv02.uchicago.edu/cgi-bin/romadict.pl?table=mcalpin&page=" + page + "'>மெக்ஆல்பின் அகராதி - David W. McAlpin Dictionary</a><table><tr><td><ul>" + content + "</ul></td></tr></table><br/>";
            }
            //winslow
            if (db[tword].w != "")
            {
                string content = "";
                string pageno = "";
                string[] row = Convert.ToString(db[tword].w).Split('ￜ');
                foreach (string i in row)
                {
                    if (i != "")
                    {
                        string[] cell = i.Split('¦');
                        if (cell[0].Substring(0, 1) == ",") { cell[0] = cell[0].Substring(1); }
                        content += "<li>" + cell[0] + "</li>";
                        pageno = cell[1];
                    }
                }
                result += "<a class='result-panel' " + styles + " target='_blank' href='http://dsalsrv02.uchicago.edu/cgi-bin/romadict.pl?table=winslow&display=utf8&page=" + pageno + "'>வின்சுலோ</a><table><tr><td><ul>" + content + "</ul></td></tr></table><br/>";
            }
            //lex
            if (db[tword].x != "")
            {
                string content = "";
                string[] row = Convert.ToString(db[tword].x).Split('ￜ');
                foreach (string i in row)
                {
                    if (i != "")
                    {
                        string[] cell = i.Split('¦');
                        if (cell[0].Substring(0, 1) == ",") { cell[0] = cell[0].Substring(1); }
                        content += "<li>" + cell[0] + "</li>";

                    }
                }
                result += "<a class='result-panel' " + styles + " target='_blank' href='http://dsalsrv02.uchicago.edu/cgi-bin/philologic/search3advanced?dbname=tamillex&query=" + Uri.EscapeUriString(tword) + "'>சென்னைப் பல்கலைக்கழகம் (DSAL)</a><table><tr><td><ul>" + content + "</ul></td></tr></table><br/>";
            }


            return result;
        }
        #endregion

        #region called on richtextbox1 keydown to capture the keypress
        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.X && e.Modifiers == Keys.Control)//ctrl+x
            {

                e.Handled = true;
                Clipboard.SetData(DataFormats.Text, richTextBox1.SelectedText);
                //string matter = richTextBox1.Rtf;
                //richTextBox1.Font = new Font(richTextBox1.Font, FontStyle.Regular);
                //richTextBox1.Cut();
                richTextBox1.SelectedText = "";
                if (richTextBox1.Text == "") { richTextBox1.Clear(); }//clear format
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)//ctrl+c
            {
                e.Handled = true;
                Clipboard.SetData(DataFormats.Text, richTextBox1.SelectedText);

            }
            else if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)//ctrl+v
            {
                e.Handled = true;
                if (Clipboard.ContainsText()) richTextBox1.SelectedText = Clipboard.GetText();
            }
        }
        #endregion

        #region called on richtextbox1 keyup to capture the keypress
        private void richTextBox1_KeyUp(object sender, KeyPressEventArgs e)
        {
            //if (toolStripComboBox1.Text != "தட்டச்சு இணக்கம்") { return; }

            int co = (int)e.KeyChar;
            
            if ((co >= 32) && (co <= 126))
            {
                e.Handled = true; //on keypress event, it skip keypress added to textbox
                //cache is not needed
                richTextBox1.SelectedText = "";//to impliment replace event
                transliterateRT(richTextBox1, e.KeyChar);
            }
            else if (co == 32)
            {
                
            }


        }
        #endregion

        #region called on richtextbox1 mouseclick to capture user mouse action
        private void richTextBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //if ((e.Location.Y < Cursor.Position.Y + 10) && (e.Location.Y < Cursor.Position.Y - 10))
            //{//avoid all mouse sodukku
            //  return;
            //}
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {  int index = richTextBox1.GetCharIndexFromPosition(e.Location);
                getsuggmenu(index);
                if (contextMenuStrip1.Visible == false) { getmenu(); }

            }
            //int index = richTextBox1.GetCharIndexFromPosition(e.Location);
            //getsuggmenu(index);
        }
        #endregion

        #region add and build suggestion in dropdown
        private void getsuggmenu(int index)
        {
            if (cache.Text == "skip") { cache.Text = ""; return; }
            cache.Text = "";
            if (contextMenuStrip1.Visible == true) { return; }
            String toSearch = richTextBox1.Text;

            if (richTextBox1.TextLength == index + 1) return;
            int leftIndex = index;

            while (leftIndex < toSearch.Count() && !Char.IsLetter(toSearch[leftIndex]))
                leftIndex++; // finds the closest word to the right

            if (leftIndex < toSearch.Count()) // did not click into whitespace at the end
            {
                while (leftIndex > 0 && istamil(toSearch[leftIndex - 1]))
                    leftIndex--;

                int rightIndex = index;

                while (rightIndex < toSearch.Count() - 1 && istamil(toSearch[rightIndex + 1]))
                    rightIndex++;
                if (rightIndex < leftIndex) { rightIndex = leftIndex + 1; }
                String word = toSearch.Substring(leftIndex, rightIndex - leftIndex + 1);


                contextMenuStrip1.Items.Clear();

                string sug = null;
                if (!String.IsNullOrEmpty(hBox.Text))
                {

                    dynamic sugword = JsonConvert.DeserializeObject(hBox.Text);
                    sug = Convert.ToString(sugword[word]);
                }
                Image image1 = Image.FromFile(@"koppu\add.jpg");
                Image image3 = Image.FromFile(@"koppu\ignore.jpg");
                if ((sug != "wrong") && (!String.IsNullOrEmpty(sug)))
                {
                    string[] sugg = sug.Split(',');
                    foreach (string item in sugg)
                    { //sBox.Items.Add(item);
                        var summa = contextMenuStrip1.Items.Add(item, (Image)null, ItemClicked);
                    }

                    Image image2 = Image.FromFile(@"koppu\replace.jpg");
                    contextMenuStrip1.Items.Add("அனைத்தும் மாற்று", image2);
                    int itemcount = contextMenuStrip1.Items.Count;
                    foreach (string item in sugg)
                    {
                        (contextMenuStrip1.Items[itemcount - 1] as ToolStripMenuItem).DropDownItems.Add(new ToolStripMenuItem(item, null, ItemClicked2));

                    }
                }
                if ((sug == "wrong") || (!String.IsNullOrEmpty(sug)))
                {
                    contextMenuStrip1.Items.Add("பிழை இல்லை", image3, ItemClicked);
                    contextMenuStrip1.Items.Add("அகராதியில் இணை", image1, ItemClicked);
                    cache.Text = Convert.ToString(leftIndex) + ',' + Convert.ToString(rightIndex);
                    contextMenuStrip1.AutoSize = false;
                    int chrcount = Convert.ToString(contextMenuStrip1.Items[0].Text).Length;
                    if (chrcount > 14) { contextMenuStrip1.Width = chrcount * 13; }
                    else
                    {
                        contextMenuStrip1.Width = 165;
                    }
                    contextMenuStrip1.Height = 24 * (contextMenuStrip1.Items.Count);
                    //contextMenuStrip1.Height//.Size ="30";

                    Point loc = Cursor.Position;// MousePosition;
                    loc.Y = loc.Y + (14 * Convert.ToInt32(richTextBox1.ZoomFactor));
                    contextMenuStrip1.Visible = true;
                    contextMenuStrip1.Show(loc);
                    //item.Click += ItemClicked;

                }
            }
            //            richTextBox1.Select(index,0);//.SelectionLength = 0;}
        }
        #endregion

        #region just read the selected words
        private void getselectedword()
        {
            if (cache.Text != "")
            {
                string[] az = cache.Text.Split(',');
                richTextBox1.Select(Convert.ToInt32(az[0]), Convert.ToInt32(az[1]) - Convert.ToInt32(az[0]) + 1);
            }
        }
        #endregion

        #region when itemclicked2
        private void ItemClicked2(object sender, EventArgs e)
        {
            getselectedword();
            string word = richTextBox1.SelectedText.Trim();
            string toword = "";
            toword = sender.ToString();
            toword = sender.ToString();
            string para = richTextBox1.Text;
            try
            {
                int n = 0;
                while (true)
                {
                    n = para.IndexOf(word, n);
                    if (n == -1) break;
                    richTextBox1.Select(n, word.Length);
                    richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size, FontStyle.Regular);
                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.SelectedText = richTextBox1.SelectedText.Replace(word, toword);

                    //richTextBox1.SelectedRtf = @"\cf2" +richTextBox1.SelectedText.Replace(word, toword);
                    para = richTextBox1.Text;
                    n = n + word.Length;
                }

            }
            catch { }
        }
        #endregion

        #region when itemclicked it displays the result
        private void ItemClicked(object sender, EventArgs e)
        {
            //string word= contextMenuStrip1..Text; 
            // MessageBox.Show(richTextBox1.SelectedRtf);
            getselectedword();
            string word = richTextBox1.SelectedText.Trim();
            string toword = "";
            try
            {//single replace
                //toword =Convert.ToString(((ToolStripLabel)sender).Text);
                toword = sender.ToString();

                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size, FontStyle.Regular);
                richTextBox1.SelectionColor = Color.Black;

                if (toword == "பிழை இல்லை")
                {
                    richTextBox1.SelectionStart = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                    richTextBox1.SelectionLength = 0;
                    checker.refreshcache(word);
                    return;
                }
                if (toword == "அகராதியில் இணை")
                {
                    richTextBox1.SelectionStart = richTextBox1.SelectionStart + richTextBox1.SelectionLength;
                    richTextBox1.SelectionLength = 0;
                    StreamReader r = new StreamReader(@"koppu\User.txt");
                    string txt = r.ReadToEnd();
                    r.Close();
                    txt = txt.Replace("\r\n", "\n");
                    string[] user = txt.Split('\n');
                    user[0] = user[0] + "," + word;
                    string matter = String.Join("\r\n", user);
                    StreamWriter file = new StreamWriter(@"koppu\User.txt");
                    file.Write(matter);
                    file.Close();
                    file.Dispose();
                    checker.refreshcache(word);
                    return;
                }
                //richTextBox1.SelectionColor = Color.Black; 
                richTextBox1.SelectedText = richTextBox1.SelectedText.Replace(word, toword);

                //richTextBox1.SelectedRtf =  richTextBox1.SelectedText.Replace(word, toword);
                //    richTextBox1.SelectedRtf = @"\cf2" +richTextBox1.SelectedText.Replace(word, toword);
            }
            catch
            {//multiple replace. those are not Toolstriplabe          

            }
            //first 

            /*string rtf1 = richTextBox1.SelectedRtf;
                rtf1= rtf1.Replace("\\ulwave", "");
            rtf1= rtf1.Replace("\\ulnone", "");
            richTextBox1.SelectedRtf = rtf1;*/

        }
        #endregion

        #region add menu items to the applications
        private void getmenu()
        {
            ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            MenuItem menuItem = new MenuItem("Cut");
            menuItem.Click += new EventHandler(CutAction);
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new MenuItem("Copy");
            menuItem.Click += new EventHandler(CopyAction);
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new MenuItem("Paste");
            menuItem.Click += new EventHandler(PasteAction);
            contextMenu.MenuItems.Add(menuItem);

            richTextBox1.ContextMenu = contextMenu;
        }
        #endregion

        #region to capture the mouseup events
        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {   //click event
                //MessageBox.Show("you got it!");
                int index = richTextBox1.GetCharIndexFromPosition(e.Location);
                getsuggmenu(index);
               // if (contextMenuStrip1.Visible == false) { getmenu(); }

            }
        }
        #endregion

        #region function will be used when cut action executed
        void CutAction(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, richTextBox1.SelectedText);
            //string matter = richTextBox1.Rtf;
            //richTextBox1.Font = new Font(richTextBox1.Font, FontStyle.Regular);
            //richTextBox1.Cut();
            richTextBox1.SelectedText = "";
            if (richTextBox1.Text == "") { richTextBox1.Clear(); }//clear format
        }
        #endregion

        #region function will be used when copy action executed
        void CopyAction(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, richTextBox1.SelectedText);
            //string matter = richTextBox1.Rtf;
            //richTextBox1.Font = new Font(richTextBox1.Font, FontStyle.Regular);
            //Clipboard.SetData(DataFormats.Rtf, richTextBox1.SelectedRtf);
            //Clipboard.Clear();
            //richTextBox1.Rtf = matter;
        }
        #endregion

        #region function will be called when paste action executed
        void PasteAction(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText()) richTextBox1.SelectedText = Clipboard.GetText();
            /*if (Clipboard.ContainsText(TextDataFormat.Rtf))
            {
                richTextBox1.SelectedRtf = Clipboard.GetData(DataFormats.Rtf).ToString();
            }*/
        }
        #endregion

        private void validate(object sender, EventArgs e)
        {
            validateRT();
        }


        #region validate the content in the richtextbox
        private void validateRT()
        {
            Cursor.Current = Cursors.WaitCursor;


            float zoom = richTextBox1.ZoomFactor;
            int sel = richTextBox1.SelectionStart;
            //System.Windows.Forms.RichTextBox rtf = new System.Windows.Forms.RichTextBox();
            //rtf.Font = new System.Drawing.Font(("Microsoft Sans Serif", System.Drawing.FontStyle.Regular);
            //string cont = richTextBox1.Text;
            //richTextBox1.Clear();//to reset format
            //richTextBox1.Text = cont;
            if (richTextBox1.Text.Length < 1) { return; }
            else//action for before correction
            {
                //panel.Visible = false;
                //hBox.Text = null;
                string aword = richTextBox1.Rtf;//All word
                aword = aword.Replace("\\ulwave", "");//Reset format
                aword = aword.Replace("\\ulw", "");
                aword = aword.Replace("\\cf0", "");
                aword = aword.Replace("\\cf1", "");
                aword = aword.Replace("\\cf2", "");
                aword = aword.Replace("\\cf3", "");
                aword = aword.Replace("\\f0", "");
                aword = aword.Replace("\\f1", "");
                aword = aword.Replace("\\f2", "");
                aword = aword.Replace("\\f3", "");
                aword = aword.Replace("\\ulnone ", "");//middle space for none
                aword = aword.Replace("\\ulnone\\", "\\");

                Regex rx = new Regex(@"\\[uU]([0-9A-F]{4})\?");
                //aword = rx.Replace(aword, match => (char.ConvertFromUtf32(Int32.Parse(match.Value.Substring(2, match.Value.Length - 3), NumberStyles.HexNumber))).ToString());
                aword = rx.Replace(aword, match => (Convert.ToChar(Int32.Parse(match.Value.Substring(2, match.Value.Length - 3)))).ToString());
                string sword = "";//splited word
                string tword = "";//tamil word
                Boolean tamil = true;
                foreach (char a in aword)
                {
                    if ((a >= 33) && (a <= 48)) { tword += "|"; sword += "|"; }//ignore comma, period
                    if ((a >= 2944) && (a <= 3071))  // if ((CharUnicodeInfo.GetNumericValue(a) >= 2944) && (CharUnicodeInfo.GetNumericValue(a) <= 3071))
                    { if (tamil) { tword += a; sword += a; } else { tword += "|" + a; sword += "|" + a; tamil = true; } }
                    else
                    { if (!tamil) { sword += a; } else { tword += "|"; sword += "|" + a; tamil = false; } }
                }

                string[] iword = sword.Split('|'); //{ "என்த","d sdfs ", "சத்துக்", " sasa as","கள்" };
                string[] tamword = tword.Split('|'); //{ "என்த","", "சத்துக்", "","கள்" };        
                dynamic[,] rword = checker.gpathil11(tamword, true, "exe");
                tamword = tword.Split('|');// assigned again since sandhi removed on tamword after gpathil10
                //button1.Text = "சம்மதம்";
                //tBox.Visible = false;
                //rBox.Visible = true;

                //rBox.Clear();
                //richTextBox1.Rtf.Text = tBox.Text;//copying whole content and then highlighting it later

                //assuming notamil in both side
                List<string> map1 = new List<String>();//To avoid duplicate and invalid names in Json
                List<string> map2 = new List<String>();
                // rBox.Font = new Font(rBox.SelectionFont, FontStyle.Regular);
                for (int i = 1; i < rword.GetLength(0); i++)//multi dimension array length of first
                {
                    if (rword[i, 1].ToString() == "wrong")
                    {
                        iword[i - 1] = iword[i - 1] + @"{\ulw";
                        iword[i + 1] = @"}" + iword[i + 1];
                        if (!map1.Contains(tamword[i]))//one unique word enters only once
                        {
                            map1.Add(tamword[i]); map2.Add(rword[i, 1].ToString());

                            //int Index = 0;
                            //int limit = rBox.Text.LastIndexOf(iword[i]);
                            //    while (Index <= limit)//for multiple repeats
                            //{
                            //                        rBox.Find(iword[i], Index, rBox.TextLength, RichTextBoxFinds.WholeWord);
                            //rBox.SelectionColor = Color.Black;
                            //rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Underline);
                            //Index = rBox.Text.IndexOf(iword[i], Index) + 1;
                            //}
                        }
                    }

                    else if ((rword[i, 1].ToString() != "correct") && (rword[i, 1].ToString() != "wrong"))
                    {
                        if (!String.IsNullOrEmpty(tamword[i]))
                        {
                            iword[i - 1] = iword[i - 1] + @"{\ulwave\cf1";
                            iword[i + 1] = @"}" + iword[i + 1];

                            if (!map1.Contains(tamword[i]))
                            {
                                map1.Add(tamword[i]); map2.Add(rword[i, 1].ToString());
                                //iword[i] = iword[i].ToString() + @"\v" + rword[i, 1].ToString() + @"\v0";

                                /*int Index = 0;
                                int limit = rBox.Text.LastIndexOf(iword[i]);
                                while (Index <= limit)//for multiple repeats
                                {
                                    rBox.Find(iword[i], Index, rBox.TextLength, RichTextBoxFinds.WholeWord);
                                    rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Regular);
                                    if ((rword[i, 1].ToString().IndexOf(",") == -1) && (autoc.Checked))
                                    {
                                        rBox.SelectionColor = Color.Green;
                                        rBox.SelectedText = rBox.SelectedText.Replace(iword[i].ToString(), rword[i, 1].ToString());
                                        Index = rBox.Text.IndexOf(rword[i, 1].ToString(), Index) + 1;
                                        if (Index == 0) { Index = limit + 1; }//to break the loop
                                    }
                                    else
                                    {
                                        rBox.SelectionColor = Color.Red;
                                        Index = rBox.Text.IndexOf(iword[i], Index) + 1;
                                    }
                                }*/
                            }
                            //rBox.SelectionColor = Color.Black;
                            //rBox.SelectionFont = new Font(rBox.SelectionFont, FontStyle.Regular);
                        }
                    }
                }
                aword = @"{\rtf1{\colortbl ;\red255\green0\blue0;\red255\green255\blue255;}" + String.Join("", iword) + "}";

                richTextBox1.Rtf = GetRtfUnicodeEscapedString(aword).ToString();

                string result = "";
                for (int i = 0; i < map1.Count; i++)
                { result = result + "\"" + map1[i] + "\":\"" + map2[i] + "\","; }
                if (result.Length > 2) { hBox.Text = "{" + result.Substring(0, result.Length - 1) + "}"; }

            }
            if (richTextBox1.ZoomFactor != zoom) { richTextBox1.ZoomFactor = zoom; }
            Cursor.Current = Cursors.Default;
            //richTextBox1.s

            //   richTextBox1.Select(sel, 0);
            // Graphics  gdi = richTextBox1.CreateGraphics();
            // gdi.DrawLine(Pens.Red, new Point(0, (richTextBox1.Lines.Length + 1) * 12), new Point(50, (richTextBox1.Lines.Length + 1) * 12));
            //gdi.Dispose();
        }
        #endregion

        #region check if the char is tamil
        public bool istamil(char a)
        {
            if ((a >= 2944) && (a <= 3071)) return true;
            return false;
        }
        #endregion

        #region check if the char is tamil 
        public bool ischar(char a)
        {
            //if (((a >= 2944) && (a <= 3071)) || ((a >= 65) && (a <= 90)) || ((a >= 97) && (a <= 122))) return true;
            if (((a >= 2944) && (a <= 3071)) || ((a >= 32) && (a <= 126))) return true;
            return false;
        }
        #endregion

        #region escape the unicode
        static string GetRtfUnicodeEscapedString(string s)
        {//converting unicode into  escape
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                //if (c == '\\' || c == '{' || c == '}')
                //   sb.Append(@"\" + c);
                //else 
                if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
            //System.Windows.Forms.RichTextBox rtf = new System.Windows.Forms.RichTextBox();
            //rtf.Text = s;
            //return rtf.Rtf
        }
        #endregion

        #region zooming button for application
        private void பரககToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.ZoomFactor < 2)
            {
                richTextBox1.ZoomFactor = richTextBox1.ZoomFactor + 0.1F;
                //   webBrowser1.Visible==true 
                if (panel3.Visible == true) setbrowsersize();
            }


        }
        #endregion

        #region set browser size
        private void setbrowsersize()
        {
            webBrowser1.Document.Body.Style = "zoom:" + Convert.ToString(richTextBox1.ZoomFactor * 100) + "%";

        }
        #endregion

        
        private void சரககToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.ZoomFactor > 0.5)
            {
                richTextBox1.ZoomFactor = richTextBox1.ZoomFactor - 0.1F;
                if (panel3.Visible == true) setbrowsersize();
            }
        }

        #region to resize the window
        private void Vaani_MinimumSizeChanged(object sender, EventArgs e)
        {
            //panel4.Size = Screen.PrimaryScreen.WorkingArea.Size;
            richTextBox1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 90);

            panel3.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            webBrowser1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 70);

            /* RECT rct;
             if (!GetWindowRect(new HandleRef(this, this.Handle), out rct))
             {        MessageBox.Show("ERROR");        return;    }
             MessageBox.Show(rct.ToString());
             richTextBox1.Left= rct.Left;
             richTextBox1.Top = rct.Top;
             richTextBox1.Width = rct.Right - rct.Left + 1;
             richTextBox1.Height = rct.Bottom - rct.Top + 1;*/
        }
        #endregion

        #region to open the tool srip menu items
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a text file";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fname = dialog.FileName;
                richTextBox1.Text = System.IO.File.ReadAllText(fname);
                richTextBox1.Font = new Font(richTextBox1.Font, FontStyle.Regular);
            }
        }
        #endregion

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save text Files";
            //saveFileDialog1.CheckFileExists = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fName = saveFileDialog1.FileName;
                StreamWriter file = new StreamWriter(fName);
                string matter = richTextBox1.Text.Replace("\n", "\r\n");
                file.Write(matter);
                file.Close();
                file.Dispose();
            }
        }

        dynamic suggs;
        string suggsi;
        private void textBox3_TextChanged(object sender, EventArgs e)
        {//call after textchanged
            fillsource();
            //   ThreadStart childref = new ThreadStart(fillsource);
            //Thread childThread = new Thread(childref);
            //childThread.Start();    
        }


        #region fill search result
        public void fillsource()
        {
            if (fsource == false) { return; }
            AutoCompleteStringCollection MyCollection = new AutoCompleteStringCollection();
            string tword = textBox3.Text;
            string initial = "";
            if (tword.Length > 0)
            {
                try
                {
                    initial = tword.Substring(0, 1);
                    if ((suggs == null) || (suggsi != initial))
                    {
                        try
                        {
                            suggs = JsonConvert.DeserializeObject(searchme(textBox3.Text, false));
                            suggsi = initial;
                        }
                        catch (System.IO.IOException e) { MessageBox.Show("unable to read"); }//when json parse error
                    }
                    MyCollection.Clear();
                    if (suggs != null) foreach (dynamic lis in suggs)
                        {
                            // count = Convert.ToString(lis.Name).IndexOf(tword);
                            if (Convert.ToString(lis.Name).IndexOf(tword) == 0)
                            {
                                MyCollection.Add(Convert.ToString(lis.Name));
                                //     suggwords += Convert.ToString(lis.Name) + ",";
                                if (MyCollection.Count > 15) 
                                    break;
                            }
                        }

                    if (MyCollection.Count > 0)
                    {
                        textBox3.AutoCompleteCustomSource = MyCollection;
                    }//System.AccessViolationException

                }
                catch { }
                //catch (System.IO.IOException e) { MessageBox.Show("unable to read"); }
            }
            else
            { suggs = null; suggsi = ""; }

        }
        #endregion

        private void settings_Click(object sender, EventArgs e)
        {

        }


        

        #region capture the textbox3 keydown and initiate the loading and searching function
        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            
            //for search in enter
            int co = (int)e.KeyCode;
            if (co == 13)
            {
                try
                {
                    if (textBox3.Text == "") return;
                    Cursor.Current = Cursors.WaitCursor;
                    // System.Threading.Thread.Sleep(1000);
                    ThreadStart childref = new ThreadStart(loading);
                    ThreadStart childref2 = new ThreadStart(searching);
                    //Console.WriteLine("In Main: Creating the Child thread");
                    // Debug.WriteLine(co);
                    Thread childThread = new Thread(childref);
                    childThread.Start();
                    Thread childThread2 = new Thread(childref2);
                    childThread2.Start();
                    //   Cursor.Current = Cursors.Default;
                }
                catch { }
            }
            //else if (((co >= 65) && (co <= 90)) || ((co >= 97) && (co <= 122)))//(co!=8)
            else if (((co >= 32) && (co <= 126)))
            {
                if (textBox3.SelectedText == textBox3.Text) { textBox3.Text = ""; }//remove selected words
                e.Handled = true;
                //  transliterate(textBox3, e.KeyCode);
            }
              Cursor.Current = Cursors.Default;
        }
        #endregion

        private void button3_Click_1(object sender, EventArgs e)
        {
            textBox3.Text = "";
            webBrowser1.DocumentText = "";
            textBox3.Focus();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            setbrowsersize();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void TName_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }
        #region just clear and focus the cursor
        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.Focus();
        }
        #endregion
        private void button4_Click(object sender, EventArgs e)
        {
            validateRT();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_DoubleClick(object sender, EventArgs e)
        {
            cache.Text = "skip";
            return;

        }
        private void rBox_Dclick(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            e.Handled = true;
            return;
        }




    }
}
#endregion
