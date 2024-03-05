﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Quests
{
    public partial class mainForm : Form
    {
        public string? currentEnv = null;
        public string? userFolder = null;
        public string? profilesFolder = null;

        public string? profilePath = null;
        public string? currentProfile = null;
        public string[] fetchedQuests;
        public string[] fetchedTraders;

        public Color listBackcolor = Color.FromArgb(255, 26, 28, 30);
        public Color listSelectedcolor = Color.FromArgb(255, 37, 37, 37);
        public Color listHovercolor = Color.FromArgb(255, 31, 33, 35);

        public Dictionary<string, string> traders = new Dictionary<string, string>();
        private Dictionary<string, string> questTranslator = new Dictionary<string, string>();

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

            if (akiServerExists && akiLauncherExists && EFTExists && userFolderExists &&  profilesFolderExists)
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
                lblQuestInfo.Text = null;

                fetchProfiles();
                Text = $"Quests (active installation: {currentEnv})";
            }
        }

        private void fetchProfiles()
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
                    lbl.Text = $"✔️ {convertedProfile}";
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
                lbl.Cursor = Cursors.Hand;
                lbl.BackColor = listBackcolor;
                lbl.ForeColor = Color.LightGray;
                lbl.Font = new Font("Bender", 13, FontStyle.Regular);
                lbl.Text = fetchedQuests[i];
                lbl.Margin = new Padding(10, 1, 1, 1);
                lbl.MouseEnter += new EventHandler(activequest_MouseEnter);
                lbl.MouseLeave += new EventHandler(activequest_MouseLeave);
                lbl.MouseDown += new MouseEventHandler(activequest_MouseDown);
                lbl.MouseUp += new MouseEventHandler(activequest_MouseUp);
                lbl.MouseDoubleClick += new MouseEventHandler(activequest_MouseDoubleClick);
                panelQuests.Controls.Add(lbl);
            }

            for (int i = 0; i < fetchedTraders.Length; i++)
            {
                Label lbl = new Label();
                lbl.Name = $"TraderItem{i}";
                lbl.AutoSize = false;
                lbl.Anchor = (AnchorStyles.Left | AnchorStyles.Top);
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Size = new Size(175, profilesPlaceholder.Size.Height);
                lbl.Location = new Point(0, profilesPlaceholder.Location.X + (i * profilesPlaceholder.Size.Height));
                lbl.Cursor = Cursors.Default;
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
                    else
                    {
                        if (Control.ModifierKeys != Keys.Control)
                        {
                            clearAll();
                        }

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
            List<string> questObjectives = new List<string>();
            string questsTemplate = File.ReadAllText(Path.Combine(currentEnv, "Aki_Data\\Server\\database\\templates\\quests.json"));
            JObject template = JObject.Parse(questsTemplate);

            string stringLocale = File.ReadAllText(Path.Combine(currentEnv, "Aki_Data\\Server\\database\\locales\\global\\en.json"));
            JObject locale = JObject.Parse(stringLocale);

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
                    questObjectives.Add(questName);
                    questObjectives.Add("");

                    JObject conditions = (JObject)item["conditions"];
                    JArray AFF = (JArray)conditions["AvailableForFinish"];
                    foreach (JObject requiredItem in (JArray)AFF)
                    {
                        string type = (string)requiredItem["_parent"];

                        if (type.ToLower() == "finditem")
                        {
                            JObject _props = (JObject)requiredItem["_props"];
                            string taskValue = (string)_props["value"];
                            JArray taskTargets = (JArray)_props["target"];

                            foreach (string target in (JArray)taskTargets)
                            {
                                string foundObject = (string)locale[$"{target} Name"];
                                questObjectives.Add($"{questTranslator[type]} {foundObject} ({taskValue})");
                            }
                        }

                        if (type.ToLower() == "handoveritem")
                        {
                            JObject _props = (JObject)requiredItem["_props"];
                            string taskValue = (string)_props["value"];
                            JArray taskTargets = (JArray)_props["target"];

                            foreach (string target in (JArray)taskTargets)
                            {
                                string foundObject = (string)locale[$"{target} Name"];
                                questObjectives.Add($"{questTranslator[type]} {foundObject} ({taskValue})");
                            }
                        }

                        if (type.ToLower() == "leaveitematlocation")
                        {
                            JObject _props = (JObject)requiredItem["_props"];
                            string taskValue = (string)_props["value"];
                            JArray taskTargets = (JArray)_props["target"];
                            string taskPlantTime = (string)_props["plantTime"];

                            foreach (string target in (JArray)taskTargets)
                            {
                                string foundObject = (string)locale[$"{target} Name"];
                                questObjectives.Add($"{questTranslator[type]} {foundObject} ({taskValue}) [{taskPlantTime} sec]");
                            }
                        }

                        if (type.ToLower() == "placebeacon")
                        {
                            JObject _props = (JObject)requiredItem["_props"];
                            string taskValue = (string)_props["value"];
                            JArray taskTargets = (JArray)_props["target"];
                            string taskPlantTime = (string)_props["plantTime"];

                            foreach (string target in (JArray)taskTargets)
                            {
                                string foundObject = (string)locale[$"{target} Name"];
                                questObjectives.Add($"{questTranslator[type]} {foundObject} ({taskValue}) [{taskPlantTime} sec]");
                            }
                        }

                        if (type.ToLower() == "skill")
                        {
                            JObject _props = (JObject)requiredItem["_props"];
                            string taskValue = (string)_props["value"];

                            JToken taskTarget = (JToken)_props["target"];

                            if (taskTarget.Type == JTokenType.Array)
                            {
                                JArray taskTargets = (JArray)_props["target"];
                                foreach (string target in (JArray)taskTargets)
                                {
                                    string foundObject = (string)locale[$"{target} Name"];
                                    questObjectives.Add($"{questTranslator[type]} {foundObject} (reach lv. {taskValue})");
                                }
                            }
                            else
                            {
                                string taskTargets = (string)_props["target"];
                                questObjectives.Add($"{questTranslator[type]} {taskTargets} (reach lv. {taskValue})");
                            }
                        }

                        if (type.ToLower() == "traderloyalty")
                        {
                            JObject _props = (JObject)requiredItem["_props"];
                            string taskValue = (string)_props["value"];
                            string target = (string)_props["target"];
                            questObjectives.Add($"{questTranslator[type]}{taskValue} with {traders[target]}");
                        }

                        if (type.ToLower() == "countercreator")
                        {
                            string foundObject = (string)locale[$"{searchQuestID} description"];
                            questObjectives.Add($"Incompatible objective (CounterCreator)");
                            questObjectives.Add(null);
                            questObjectives.Add(foundObject);
                            break;
                            // JObject _props = (JObject)requiredItem["_props"];
                            // string taskValue = (string)_props["value"];
                            // string target = (string)_props["target"];
                            // questObjectives.Add($"{questTranslator[type]}{taskValue} with {traders[target]}");
                        }
                    }

                    break;
                }
            }

            string compiled = string.Join(Environment.NewLine, questObjectives);
            lblQuestInfo.Text = compiled;
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
    }
}