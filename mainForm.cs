using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Font = System.Drawing.Font;

namespace Quests
{
    public partial class mainForm : Form
    {
        public string? currentEnv = Environment.CurrentDirectory;
        public string? userFolder = null;
        public string? profilesFolder = null;

        public string? profilePath = null;
        public string? currentProfile = null;
        public string? selectedSubTask = null;
        public string? selectedSubTaskValue = null;
        public string[] fetchedQuests;
        public string[] fetchedTraders;

        public Color listBackcolor = Color.FromArgb(255, 26, 28, 30);
        public Color listSelectedcolor = Color.FromArgb(255, 37, 37, 37);
        public Color listHovercolor = Color.FromArgb(255, 31, 33, 35);

        public Dictionary<string, string> traders = new Dictionary<string, string>();
        private Dictionary<string, string> questTranslator = new Dictionary<string, string>();

        public bool? isDescLoaded = false;
        public bool isDevMode = false;
        private Form descForm;
        private messageWindow msgBoard;

        public mainForm()
        {
            InitializeComponent();
        }

        private void mainForm_Load(object sender, EventArgs e)
        {
            bool isDirectorySPT = checkSPTFiles();
            if (!isDirectorySPT)
            {
                string pathFile = Path.Combine(currentEnv, "path.txt");
                bool pathFileExists = File.Exists(pathFile);
                if (pathFileExists)
                {
                    currentEnv = File.ReadAllText(pathFile);
                    userFolder = Path.Combine(currentEnv, "user");
                    profilesFolder = Path.Combine(userFolder, "profiles");
                }
            }
            else
                currentEnv = Environment.CurrentDirectory;

            startFetching();
        }

        private bool checkSPTFiles()
        {
            string akiServer = Path.Combine(currentEnv, "Aki.Server.exe");
            string akiLauncher = Path.Combine(currentEnv, "Aki.Launcher.exe");
            string EFT = Path.Combine(currentEnv, "EscapeFromTarkov.exe");

            userFolder = Path.Combine(currentEnv, "user");
            profilesFolder = Path.Combine(userFolder, "profiles");

            bool akiServerExists = File.Exists(akiServer);
            bool akiLauncherExists = File.Exists(akiLauncher);
            bool EFTExists = File.Exists(EFT);
            bool userFolderExists = File.Exists(userFolder);
            bool profilesFolderExists = File.Exists(profilesFolder);

            if (akiServerExists && akiLauncherExists && EFTExists && userFolderExists && profilesFolderExists)
            {
                return true;
            }

            return false;
        }

        private void startFetching()
        {
            bool userFolderExists = Directory.Exists(userFolder);
            bool profilesFolderExists = Directory.Exists(profilesFolder);

            if (userFolderExists && profilesFolderExists)
            {
                traders.Clear();
                questTranslator.Clear();

                panelQuests.Controls.Clear();
                panelProfiles.BringToFront();

                if (descForm != null)
                {
                    descForm.Close();
                }

                listProfiles();
                Text = $"Quests (active installation: {currentEnv})";
            }
        }

        private void switchDevMode(Control pastLbl)
        {
            string name = null;

            foreach (Control c in panelQuests.Controls)
            {
                if (c is Label lbl)
                {
                    if (lbl.Text.ToLower().Contains("developer mode"))
                    {
                        name = lbl.Name;
                    }


                    if (lbl.Text.ToLower() == "◻️ developer mode" && lbl.Name == name)
                    {
                        lbl.Text = "◼️ Developer Mode";
                        isDevMode = true;
                        break;
                    }
                    else if (lbl.Text.ToLower() == "◼️ developer mode" && lbl.Name == name)
                    {
                        lbl.Text = "◻️ Developer Mode";
                        isDevMode = false;
                        break;
                    }

                    readQuestDetails(pastLbl.Text, pastLbl);
                }
            }
        }

        private void insertItem(RichTextBox origin, string item, string valueType, bool isDigit)
        {
            Font font = new Font("Bender", 15, FontStyle.Bold);
            Font otherFont = new Font("Bender", 11, FontStyle.Bold);

            if (valueType != null)
            {
                switch (valueType.ToLower())
                {
                    case "min":
                        if (isDigit)
                            origin.SelectionFont = font;
                        origin.SelectionColor = Color.IndianRed;
                        origin.AppendText(item + Environment.NewLine);
                        origin.SelectionColor = origin.ForeColor;
                        if (isDigit)
                            origin.SelectionFont = otherFont;
                        break;
                    case "between":
                        if (isDigit)
                            origin.SelectionFont = font;
                        origin.SelectionColor = Color.DodgerBlue;
                        origin.AppendText(item + Environment.NewLine);
                        origin.SelectionColor = origin.ForeColor;
                        if (isDigit)
                            origin.SelectionFont = otherFont;
                        break;
                    case "threshold":
                        if (isDigit)
                            origin.SelectionFont = font;
                        origin.SelectionColor = Color.MediumSpringGreen;
                        origin.AppendText(item + Environment.NewLine);
                        origin.SelectionColor = origin.ForeColor;
                        if (isDigit)
                            origin.SelectionFont = otherFont;
                        break;
                }
            }
            else
            {
                origin.SelectionColor = origin.ForeColor;
                origin.AppendText(item + Environment.NewLine);
                origin.SelectionColor = origin.ForeColor;
            }
        }

        private void setLineFont(RichTextBox rtb, int lineIndex, int fontSize, bool questInfo)
        {
            if (questInfo)
            {
                int lineStartIndex = rtb.GetFirstCharIndexFromLine(lineIndex);
                int lineEndIndex = rtb.GetFirstCharIndexFromLine(lineIndex + 1);
                if (lineEndIndex == -1)
                {
                    lineEndIndex = rtb.TextLength;
                }

                rtb.Select(lineStartIndex, lineEndIndex - lineStartIndex);
                rtb.SelectionFont = new Font("Bender", fontSize, FontStyle.Bold); ;
                rtb.Select(0, 0);
            }
            else
            {
                int lineStartIndex = rtb.GetFirstCharIndexFromLine(lineIndex);
                int lineEndIndex = rtb.GetFirstCharIndexFromLine(lineIndex + 1);
                if (lineEndIndex == -1)
                {
                    lineEndIndex = rtb.TextLength;
                }

                rtb.Select(lineStartIndex, lineEndIndex - lineStartIndex);
                rtb.SelectionFont = new Font("Bender", fontSize, FontStyle.Bold); ;
                rtb.Select(0, 0);
            }
        }

        private void listProfiles()
        {
            string[] profiles = Directory.GetFiles(profilesFolder, "*.json");
            for (int i = 0; i < profiles.Length; i++)
            {
                Label lbl = new Label();
                lbl.Name = $"accountProfile{i}";
                lbl.AutoSize = false;
                lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Size = new Size(profilesPlaceholder.Size.Width, profilesPlaceholder.Size.Height);
                lbl.Location = new Point(profilesPlaceholder.Location.X, profilesPlaceholder.Location.Y + (i * profilesPlaceholder.Size.Height));
                lbl.Font = new Font("Bender", 13, FontStyle.Regular);
                lbl.BackColor = panelProfiles.BackColor;
                lbl.ForeColor = Color.LightGray;
                lbl.Padding = new Padding(10, 0, 0, 0);
                lbl.Cursor = Cursors.Hand;
                lbl.MouseEnter += new EventHandler(profile_MouseEnter);
                lbl.MouseLeave += new EventHandler(profile_MouseLeave);
                lbl.MouseDown += new MouseEventHandler(profile_MouseDown);
                lbl.MouseUp += new MouseEventHandler(profile_MouseUp);

                string convertedProfile = convertProfile(profiles[i]);
                if (convertedProfile != null || convertedProfile != "")
                {
                    string aid = Path.GetFileNameWithoutExtension(profiles[i]);
                    string profileStats = displayProfileData(profiles[i]);
                    lbl.Text = $"✔️ {convertedProfile} {profileStats}";
                    lbl.Tag = aid;
                }
                else
                {
                    lbl.Text = $"✔️ Incomplete profile";
                }

                panelProfiles.Controls.Add(lbl);
            }
        }

        private void profile_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                string cleanOutput = label.Text.Replace("✔️ ", "");
                currentProfile = cleanOutput;

                string AID = label.Tag.ToString();
                fetchQuests(AID);
                displayQuestWindow();
            }
        }

        private void profile_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.BackColor = listHovercolor;
            }
        }

        private void profile_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.BackColor = listBackcolor;
                label.Invalidate();
            }
        }

        private void profile_MouseUp(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                label.BackColor = listHovercolor;
                label.Invalidate();
            }
        }

        private string convertProfile(string AID)
        {
            string cleanAID = Path.GetFileNameWithoutExtension(AID);
            string fullAID = Path.Combine(profilesFolder, $"{cleanAID}.json");

            bool fullAIDExists = File.Exists(fullAID);
            if (fullAIDExists)
            {
                string fileContent = File.ReadAllText(fullAID);
                JObject parsedFile = JObject.Parse(fileContent);
                JObject characters = (JObject)parsedFile["characters"];
                JObject info = (JObject)parsedFile["info"];

                if (characters.Type != JTokenType.Null)
                {
                    JObject pmc = (JObject)characters["pmc"];

                    if (pmc.Type != JTokenType.Null)
                    {
                        JObject Info = (JObject)pmc["Info"];
                        if (Info != null)
                        {
                            string Nickname = (string)Info["Nickname"];
                            string infoAID = (string)info["id"];

                            if (infoAID == cleanAID)
                            {
                                return Nickname;
                            }
                        }
                    }
                }
            }
            return "Incomplete profile";
        }

        private string displayProfileData(string AID)
        {
            string cleanAID = Path.GetFileNameWithoutExtension(AID);
            string fullAID = Path.Combine(profilesFolder, $"{cleanAID}.json");

            bool fullAIDExists = File.Exists(fullAID);
            if (fullAIDExists)
            {
                string fileContent = File.ReadAllText(fullAID);
                JObject parsedFile = JObject.Parse(fileContent);
                JObject characters = (JObject)parsedFile["characters"];
                JObject info = (JObject)parsedFile["info"];

                if (characters.Type != JTokenType.Null)
                {
                    JObject pmc = (JObject)characters["pmc"];

                    if (pmc.Type != JTokenType.Null)
                    {
                        JObject Info = (JObject)pmc["Info"];
                        if (Info != null)
                        {
                            string Side = (string)Info["Side"];
                            int Level = (int)Info["Level"];
                            string infoAID = (string)info["id"];

                            if (infoAID == cleanAID)
                            {
                                return $" ({Side.ToUpper()} lvl {Level.ToString()})";
                            }
                        }
                    }
                }
            }
            return "Incomplete profile";
        }

        private void fetchQuests(string profileAID)
        {
            fetchedQuests = new string[0];
            fetchedTraders = new string[0];

            traders.Add("5a7c2eca46aef81a7ca2145d", "Mechanic");
            traders.Add("5ac3b934156ae10c4430e83c", "Ragman");
            traders.Add("5c0647fdd443bc2504c2d371", "Jaeger");
            traders.Add("54cb50c76803fa8b248b4571", "Prapor");
            traders.Add("54cb57776803fa99248b456e", "Therapist");
            traders.Add("579dc571d53a0658a154fbec", "Fence");
            traders.Add("638f541a29ffd1183d187f57", "Lightkeeper");
            traders.Add("5935c25fb3acc3127c3d8cd9", "Peacekeeper");
            traders.Add("58330581ace78e27b8b10cee", "Skier");

            questTranslator.Add("HandoverItem", "Hand over");
            questTranslator.Add("FindItem", "Obtain");
            questTranslator.Add("LeaveItemAtLocation", "Stash");
            questTranslator.Add("PlaceBeacon", "Plant");
            questTranslator.Add("Skill", "Level up");
            questTranslator.Add("TraderLoyalty", "Reach LL");
            questTranslator.Add("Exploration", "Scout");
            questTranslator.Add("Elimination", "Kill");

            if (Directory.Exists(userFolder))
            {
                if (Directory.Exists(profilesFolder))
                {
                    string currentProfile = Path.Combine(profilesFolder, $"{profileAID}.json");
                    if (File.Exists(currentProfile))
                    {
                        profilePath = currentProfile;
                        readProfileQuests(currentProfile);
                    }
                }
            }
        }

        private void displayQuestWindow()
        {
            int innerMargin = 15;

            if (descForm != null)
            {
                descForm.Close();
                descForm.Dispose();
            }

            descForm = new Form();
            descForm.Text = "Quest information";
            descForm.Size = new Size(this.Size.Width * 2 / 3 + 100, this.Size.Height);
            descForm.Font = new Font("Bender", 9, FontStyle.Regular);
            descForm.BackColor = listBackcolor;
            descForm.ForeColor = Color.LightGray;
            descForm.ControlBox = true;
            descForm.MinimizeBox = false;
            descForm.MaximizeBox = false;
            descForm.MinimumSize = new Size(300, this.Size.Height);
            descForm.Resize += questDesc_Resize;

            RichTextBox questLbl = new RichTextBox();
            questLbl.Name = $"questLbl";
            questLbl.AutoSize = false;
            questLbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);
            questLbl.BorderStyle = BorderStyle.None;
            questLbl.Size = new Size(descForm.Size.Width - innerMargin, descForm.Size.Height - 50);
            questLbl.Location = new Point(innerMargin, innerMargin);
            questLbl.Font = new Font("Bender", 11, FontStyle.Bold);
            questLbl.BackColor = descForm.BackColor;
            questLbl.ForeColor = Color.LightGray;
            questLbl.Cursor = Cursors.IBeam;
            questLbl.WordWrap = true;
            questLbl.Multiline = true;
            questLbl.MouseWheel += questDesc_MouseWheel;
            questLbl.MouseDown += questDesc_MouseDown;

            descForm.Show();
            descForm.Controls.Add(questLbl);

            UpdateSecondFormPosition();
        }

        private Control returnQuest()
        {
            foreach (Control control in panelQuests.Controls)
            {
                if (control is Label lbl)
                {
                    if (lbl.Name.ToLower().StartsWith("questitem") && lbl.ForeColor == Color.DodgerBlue)
                    {
                        return lbl;
                    }
                }
            }
            return null;
        }

        private void clearAll()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Label lbl)
                {
                    if (lbl.Name.ToLower() != "btnimporttxt")
                    {
                        lbl.BackColor = listBackcolor;
                        lbl.ForeColor = Color.LightGray;
                    }
                }
            }
        }

        private void listQuests()
        {
            int fullLength = 0;

            for (int i = 0; i < fetchedQuests.Length; i++)
            {
                fullLength++;

                Label lbl = new Label();
                lbl.Name = $"QuestItem{i}";
                lbl.AutoSize = false;
                lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Size = new Size(panelQuests.Size.Width, profilesPlaceholder.Size.Height);
                lbl.Location = new Point(175, profilesPlaceholder.Location.X + (i * profilesPlaceholder.Size.Height));
                lbl.BackColor = listBackcolor;
                lbl.ForeColor = Color.LightGray;
                lbl.Font = new Font("Bender", 13, FontStyle.Regular);
                lbl.Text = fetchedQuests[i];
                lbl.Margin = new Padding(10, 1, 1, 1);
                if (fetchedQuests != null)
                {
                    lbl.MouseEnter += new EventHandler(activequest_MouseEnter);
                    lbl.MouseLeave += new EventHandler(activequest_MouseLeave);
                    lbl.MouseDown += new MouseEventHandler(activequest_MouseDown);
                    lbl.MouseUp += new MouseEventHandler(activequest_MouseUp);
                    lbl.MouseDoubleClick += new MouseEventHandler(activequest_MouseDoubleClick);
                    lbl.Cursor = Cursors.Hand;
                }
                else
                {
                    lbl.Cursor = Cursors.Default;
                }

                panelQuests.Controls.Add(lbl);
            }

            for (int i = 0; i < fetchedTraders.Length; i++)
            {
                Label lbl = new Label();

                if (fetchedTraders[i] != null)
                {
                    if (fetchedTraders[i].ToLower().Contains("developer mode"))
                    {
                        lbl.Name = $"QuestItem{i}";
                        lbl.Cursor = Cursors.Hand;
                    }
                    else
                    {
                        lbl.Name = $"TraderItem{i}";
                        lbl.Cursor = Cursors.Default;
                    }

                    lbl.AutoSize = false;
                    lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    lbl.Size = new Size(175, profilesPlaceholder.Size.Height);
                    lbl.Location = new Point(0, profilesPlaceholder.Location.X + (i * profilesPlaceholder.Size.Height));
                    lbl.BackColor = listBackcolor;
                    lbl.ForeColor = Color.LightGray;
                    lbl.Font = new Font("Bender", 9, FontStyle.Regular);
                    lbl.Text = fetchedTraders[i];
                    lbl.Margin = new Padding(10, 1, 1, 1);
                    lbl.MouseEnter += new EventHandler(activequest_MouseEnter);
                    lbl.MouseLeave += new EventHandler(activequest_MouseLeave);
                    lbl.MouseDown += new MouseEventHandler(activequest_MouseDown);
                    lbl.MouseUp += new MouseEventHandler(activequest_MouseUp);
                    panelQuests.Controls.Add(lbl);
                }
            }
        }

        private void selectQuestAsComplete(string fetchQuest)
        {
            string questsTemplate = File.ReadAllText(Path.Combine(currentEnv, "Aki_Data\\Server\\database\\templates\\quests.json"));
            JObject template = JObject.Parse(questsTemplate);

            string fullProfileRead = File.ReadAllText(profilePath);
            JObject profile = JObject.Parse(fullProfileRead);
            int activeQuests = 0;

            string searchQuestID = "";
            string searchQuestName = "";

            foreach (var questItem in template.Properties())
            {
                JObject item = (JObject)questItem.Value;
                string questName = (string)item["QuestName"]?.Value<string>();
                string traderId = (string)item["traderId"]?.Value<string>();
                string questID = (string)item["_id"]?.Value<string>();

                if (questName == fetchQuest)
                {
                    searchQuestID = questID;
                    searchQuestName = questName;
                    break;
                }
            }

            if (profile.ContainsKey("characters"))
            {
                JObject characters = (JObject)profile["characters"];
                if (characters.ContainsKey("pmc"))
                {
                    JObject pmc = (JObject)characters["pmc"];
                    if (pmc.ContainsKey("Quests"))
                    {
                        if (pmc["Quests"] is JArray quests)
                        {
                            foreach (JObject fetchedQuest in (JArray)quests)
                            {
                                if (fetchedQuest.ContainsKey("qid"))
                                {
                                    string QID = (string)fetchedQuest["qid"];
                                    if (QID == searchQuestID)
                                    {
                                        fetchedQuest["status"] = 3;
                                        string updatedJSON = profile.ToString(Formatting.Indented);
                                        File.WriteAllText(profilePath, updatedJSON);
                                        MessageBox.Show($"Quest {searchQuestName} set to Completed.");
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            readProfileQuests(profilePath);
        }

        private void arrInsert(string item)
        {
            Array.Resize(ref fetchedQuests, fetchedQuests.Length + 1);
            fetchedQuests[fetchedQuests.Length - 1] = item;
        }

        private void arr2Insert(string item)
        {
            Array.Resize(ref fetchedTraders, fetchedTraders.Length + 1);
            fetchedTraders[fetchedTraders.Length - 1] = item;
        }

        private void activequest_MouseEnter(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                if (label.Name.StartsWith("QuestItem"))
                {
                    if (label.BackColor == listSelectedcolor)
                        return;
                    else
                        label.BackColor = listHovercolor;

                }
                else
                {
                    return;
                }
            }
        }

        private void activequest_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                if (label.Name.StartsWith("QuestItem"))
                {
                    Control questLabel = returnQuest();
                    if (questLabel != null)
                    {
                        string fetchQuest = questLabel.Text;
                        selectQuestAsComplete(fetchQuest);
                        readProfileQuests(profilePath);
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void activequest_MouseLeave(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                if (label.Name.StartsWith("QuestItem"))
                {
                    if (label.BackColor == listSelectedcolor)
                        return;
                    else
                        label.BackColor = listBackcolor;
                }
                else
                {
                    return;
                }
            }
        }

        private void activequest_MouseUp(object sender, EventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                // label.BackColor = listHovercolor;
            }
        }

        private void activequest_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.Label label = (System.Windows.Forms.Label)sender;
            if (label.Text != "")
            {
                foreach (Control ctrl in panelQuests.Controls)
                {
                    if (ctrl is Label lbl)
                    {
                        lbl.BackColor = listBackcolor;
                        lbl.ForeColor = Color.LightGray;
                    }
                }

                if (label.Name.StartsWith("QuestItem"))
                {
                    if (label.Text.ToLower().Contains("back to profiles"))
                    {
                        startFetching();
                    }
                    else if (label.Text.ToLower().Contains("developer mode"))
                    {
                        switchDevMode(label);

                        if (descForm != null)
                        {
                            displayQuestWindow();
                        }
                    }
                    else
                    {
                        if (Control.ModifierKeys != Keys.Control)
                        {
                            clearAll();
                        }

                        if (descForm != null)
                        {
                            displayQuestWindow();
                        }

                        selectedSubTask = null;
                        selectedSubTaskValue = null;

                        label.BackColor = listHovercolor;
                        label.ForeColor = Color.DodgerBlue;
                        readQuestDetails(label.Text, label);

                        int questItemNmr = Convert.ToInt32(label.Name.Substring(label.Name.Length - 1));
                        Control traderItem = panelQuests.Controls[$"TraderItem{questItemNmr}"];
                        if (traderItem != null)
                        {
                            traderItem.ForeColor = Color.DodgerBlue;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void readProfileQuests(string path)
        {
            fetchedQuests = new string[0];
            fetchedTraders = new string[0];

            arrInsert("⬅️ Back to profiles");
            arrInsert(null);

            arr2Insert("◻️ Developer Mode");
            arr2Insert(null);

            string profileName = "";
            int profileLvl = 0;

            string questsTemplate = File.ReadAllText(Path.Combine(currentEnv, "Aki_Data\\Server\\database\\templates\\quests.json"));
            JObject template = JObject.Parse(questsTemplate);

            string locales = File.ReadAllText(Path.Combine(currentEnv, "Aki_Data\\Server\\database\\locales\\global\\en.json"));
            JObject locale = JObject.Parse(locales);

            string fullProfileRead = File.ReadAllText(path);
            JObject profile = JObject.Parse(fullProfileRead);
            int activeQuests = 0;

            if (profile.ContainsKey("characters"))
            {
                JObject characters = (JObject)profile["characters"];
                if (characters.ContainsKey("pmc"))
                {
                    JObject pmc = (JObject)characters["pmc"];

                    if (pmc.ContainsKey("Info"))
                    {
                        JObject info = (JObject)pmc["Info"];
                        profileName = (string)info["Nickname"];
                        profileLvl = (int)info["Level"];
                    }

                    if (pmc.ContainsKey("Quests"))
                    {
                        if (pmc["Quests"] is JArray quests)
                        {
                            bool noQuestsStarted = quests.All(item =>
                            {
                                if (item is JObject obj && obj.TryGetValue("status", out JToken statusToken))
                                {
                                    return statusToken.ToString() == "Locked";
                                }
                                return false;
                            });
                            if (!noQuestsStarted)
                            {
                                panelQuests.Controls.Clear();
                                panelProfiles.Controls.Clear();
                                panelQuests.BringToFront();

                                foreach (var quest in quests)
                                {
                                    int started = 2;
                                    if (quest["status"].Value<int>() == started)
                                    {
                                        foreach (var questItem in template.Properties())
                                        {
                                            JObject item = (JObject)questItem.Value;
                                            string questName = (string)item["QuestName"]?.Value<string>();
                                            string traderId = (string)item["traderId"]?.Value<string>();
                                            string questID = (string)item["_id"]?.Value<string>();
                                            string questLoc = (string)item["location"]?.Value<string>();

                                            string localeLoc = (string)locale[$"{questLoc} Name"]?.Value<string>();

                                            if (questID == quest["qid"]?.Value<string>())
                                            {
                                                arrInsert(questName);
                                                arr2Insert($"{traders[traderId]} ({localeLoc})");
                                                activeQuests++;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show($"{profileName} has not started any quests, they are all locked", this.Text, MessageBoxButtons.OK);
                                startFetching();
                            }
                        }
                    }
                }
            }
            listQuests();
        }

        private void readQuestDetails(string fetchQuest, Control originalLbl)
        {
            string devModeCompiled = null;
            List<string> questObjectives = new List<string>();
            List<string> questSubObjectives = new List<string>();
            string questsTemplate = File.ReadAllText(Path.Combine(currentEnv, "Aki_Data\\Server\\database\\templates\\quests.json"));
            JObject template = JObject.Parse(questsTemplate);

            string stringLocale = File.ReadAllText(Path.Combine(currentEnv, "Aki_Data\\Server\\database\\locales\\global\\en.json"));
            JObject locale = JObject.Parse(stringLocale);

            string fullProfileRead = File.ReadAllText(profilePath);
            JObject profile = JObject.Parse(fullProfileRead);
            int activeQuests = 0;
            RichTextBox questLbl = descForm.Controls.Find("questLbl", false).FirstOrDefault() as RichTextBox;

            string searchQuestID = null;
            string searchQuestName = null;
            string searchQuestTraderId = null;
            bool isExitStatus = false;


            if (questLbl != null)
            {
                foreach (var questItem in template.Properties())
                {
                    JObject item = (JObject)questItem.Value;
                    string questName = (string)item["QuestName"]?.Value<string>();
                    string traderId = (string)item["traderId"]?.Value<string>();
                    string questID = (string)item["_id"]?.Value<string>();
                    searchQuestName = questName;
                    searchQuestTraderId = traderId;

                    if (questName == fetchQuest)
                    {
                        searchQuestID = questID;
                        searchQuestName = questName;
                        questObjectives.Add("");

                        insertItem(questLbl, searchQuestName, null, false);
                        insertItem(questLbl, traders[searchQuestTraderId], null, false);
                        insertItem(questLbl, null, null, false);

                        JObject conditions = (JObject)item["conditions"];
                        JArray AFF = (JArray)conditions["AvailableForFinish"];

                        if (isDevMode)
                        {
                            devModeCompiled = item.ToString();
                        }
                        else
                        {
                            foreach (JObject requiredItem in (JArray)AFF)
                            {
                                string conditionType = (string)requiredItem["conditionType"];
                                string objectiveId = (string)requiredItem["id"];
                                int objectiveValue = (int)requiredItem["value"];
                                string foundObject = (string)locale[objectiveId];

                                //string total = $"{foundObject} ({objectiveValue.ToString()})";

                                string profileContent = File.ReadAllText(profilePath);
                                JObject profileObj = JObject.Parse(profileContent);
                                JObject characters = (JObject)profileObj["characters"];
                                JObject pmc = (JObject)characters["pmc"];
                                JObject TaskConditionCounters = (JObject)pmc["TaskConditionCounters"];

                                var results = TaskConditionCounters.Properties()
                                    .Where(p => p.Value["id"] != null && (string)p.Value["id"] == objectiveId);

                                foreach (var result in results)
                                {
                                    JObject resultParent = (JObject)result.Value;
                                    string id = (string)resultParent["id"];
                                    int val = (int)resultParent["value"];
                                    string questString = (string)locale[id];

                                    int minValue = 0;
                                    int maxValue = objectiveValue;

                                    if (val == 0)
                                    {
                                        insertItem(questLbl, foundObject, "min", false);
                                        insertItem(questLbl, $" {val.ToString()} / {objectiveValue.ToString()}", "min", true);
                                        insertItem(questLbl, $"", null, false);
                                    }
                                    else if (val > minValue && val < maxValue)
                                    {
                                        insertItem(questLbl, foundObject, "between", false);
                                        insertItem(questLbl, $" {val.ToString()} / {objectiveValue.ToString()}", "between", true);
                                        insertItem(questLbl, $"", null, false);
                                    }
                                    else
                                    {
                                        insertItem(questLbl, foundObject, "threshold", false);
                                        // insertItem(questLbl, $"{val.ToString()} / {objectiveValue.ToString()}", "threshold", true);
                                        insertItem(questLbl, $" [ ✔️ ]", "threshold", true);
                                        insertItem(questLbl, $"", null, false);
                                    }
                                }
                            }
                        }

                        setLineFont(questLbl, 0, 22, true);
                        setLineFont(questLbl, 1, 15, true);
                        break;
                    }
                }
            }

            if (isDevMode)
            {
                if (questLbl != null)
                {
                    string prefix = searchQuestName + Environment.NewLine + Environment.NewLine +
                                    $"Trader: {traders[searchQuestTraderId]}" + Environment.NewLine;
                    questLbl.Text = prefix + Environment.NewLine + devModeCompiled;
                }
            }
            else
            {
                /*
                string prefix = searchQuestName + Environment.NewLine + Environment.NewLine +
                                   $"Trader: {traders[searchQuestTraderId]}" + Environment.NewLine;
                string compiled = prefix + string.Join(Environment.NewLine, questObjectives);
                Control questLbl = descForm.Controls.Find("questLbl", false).FirstOrDefault();
                if (questLbl != null)
                {
                    questLbl.Text = compiled;
                }
                */
            }
        }

        private void mainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Control questLabel = returnQuest();
                if (questLabel != null)
                {
                    string fetchQuest = questLabel.Text;
                    selectQuestAsComplete(fetchQuest);
                }
            }
        }

        private void UpdateSecondFormPosition()
        {
            if (descForm != null)
            {
                descForm.Location = new Point(this.Right - 15, this.Top);
            }
        }

        private void mainForm_LocationChanged(object sender, EventArgs e)
        {
            UpdateSecondFormPosition();
        }

        private void mainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                if (descForm != null)
                {
                    descForm.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void questComplete_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            if (btn != null)
            {
                string content = $"Complete task? You will receive all rewards" + Environment.NewLine + Environment.NewLine +
                                 $"{selectedSubTask}" + Environment.NewLine +
                                 $"{selectedSubTaskValue}  ➞  [ ✔️ ]";

                msgBoard = new messageWindow();
                msgBoard.messageText.Text = content;

                msgBoard.ShowDialog();
            }
        }

        private void questSkip_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            if (btn != null)
            {
                string content = $"Skip task? You will receive no rewards" + Environment.NewLine + Environment.NewLine +
                                 $"{selectedSubTask}" + Environment.NewLine +
                                 $"{selectedSubTaskValue}  ➞  [ ➖ ]";

                msgBoard = new messageWindow();
                msgBoard.messageText.Text = content;

                msgBoard.ShowDialog();
            }
        }

        private void questDesc_Resize(object sender, EventArgs e)
        {
            if (descForm != null)
            {
                if (descForm.WindowState == FormWindowState.Normal)
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }

        private void questDesc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //
            }
        }

        private void questDesc_MouseWheel(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)sender;

            if (e.Delta > 0)
            {
                // down
                if (rtb.Location.X > 30)
                    return;
                else
                {
                    rtb.Location = new Point(rtb.Location.X + 1, rtb.Location.Y + 1);
                    rtb.Size = new Size(rtb.Size.Width - 1, rtb.Size.Height - 1);
                }
            }
            else
            {
                // up
                if (rtb.Location.X == 1)
                    return;
                else
                {
                    rtb.Location = new Point(rtb.Location.X - 1, rtb.Location.Y - 1);
                    rtb.Size = new Size(rtb.Size.Width + 1, rtb.Size.Height + 1);
                }
            }
        }

        private void questDesc_MouseDown(object sender, MouseEventArgs e)
        {
            System.Windows.Forms.RichTextBox rtb = (System.Windows.Forms.RichTextBox)sender;
            if (descForm != null)
            {
                int innerMargin = 15;
                descForm.Text = "Quests";

                Button questCompletor = panelProfiles.Controls.Find("questCompletor", false).FirstOrDefault() as Button;
                Button questSkipper = panelProfiles.Controls.Find("questSkipper", false).FirstOrDefault() as Button;

                if (questCompletor != null)
                {
                    panelProfiles.Controls.Remove(questCompletor);
                    questCompletor.Dispose();
                }

                if (questSkipper != null)
                {
                    panelProfiles.Controls.Remove(questSkipper);
                    questSkipper.Dispose();
                }

                int currentLineIndex = rtb.GetLineFromCharIndex(rtb.SelectionStart);
                string currentLineText = rtb.Lines[currentLineIndex];

                if (currentLineIndex >= 3)
                {
                    if (!string.IsNullOrEmpty(currentLineText))
                    {
                        // line has text
                        if (char.IsLetter(currentLineText[0]))
                        {
                            Color caretColor = rtb.SelectionColor;
                            if (caretColor != Color.MediumSpringGreen)
                            {
                                string counterLinetext = rtb.Lines[currentLineIndex + 1];
                                selectedSubTask = currentLineText;
                                selectedSubTaskValue = counterLinetext;

                                questCompletor = new Button();
                                questCompletor.Name = $"questCompletor";
                                questCompletor.AutoSize = false;
                                questCompletor.Cursor = Cursors.Hand;
                                questCompletor.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
                                questCompletor.Location = new Point(innerMargin, panelProfiles.Size.Height - 60);
                                questCompletor.FlatStyle = FlatStyle.Flat;
                                questCompletor.FlatAppearance.BorderSize = 1;
                                questCompletor.FlatAppearance.BorderColor = Color.FromArgb(255, 100, 100, 100);
                                questCompletor.Size = new Size(150, 45);
                                questCompletor.Font = new Font("Bender", 11, FontStyle.Bold);
                                questCompletor.BackColor = descForm.BackColor;
                                questCompletor.ForeColor = Color.LightGray;
                                questCompletor.Text = "Complete task";

                                questSkipper = new Button();
                                questSkipper.Name = $"questSkipper";
                                questSkipper.AutoSize = false;
                                questSkipper.Cursor = Cursors.Hand;
                                questSkipper.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
                                questSkipper.Location = new Point(questCompletor.Size.Width + innerMargin * 2, panelProfiles.Size.Height - 60);
                                questSkipper.FlatStyle = FlatStyle.Flat;
                                questSkipper.FlatAppearance.BorderSize = 1;
                                questSkipper.FlatAppearance.BorderColor = Color.FromArgb(255, 100, 100, 100);
                                questSkipper.Size = new Size(150, 45);
                                questSkipper.Font = new Font("Bender", 11, FontStyle.Bold);
                                questSkipper.BackColor = descForm.BackColor;
                                questSkipper.ForeColor = Color.LightGray;
                                questSkipper.Text = "Skip task";

                                questCompletor.Click += questComplete_Click;
                                questSkipper.Click += questSkip_Click;

                                panelProfiles.Controls.Add(questCompletor);
                                panelProfiles.Controls.Add(questSkipper);
                            }
                        }
                    }
                }
            }
        }

        private void mainForm_Shown(object sender, EventArgs e)
        {
            if (descForm != null)
            {
                descForm.Show();

                if (msgBoard != null)
                    msgBoard.Show();
            }
        }
    }
}
