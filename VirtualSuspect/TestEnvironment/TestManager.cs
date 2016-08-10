﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualSuspect;
using VirtualSuspect.KnowledgeBase;
using VirtualSuspect.Utils;

namespace TestEnvironment
{
    public static class TestManager{

        private static Dictionary<int,TestSuspect> testSuspects = new Dictionary<int, TestSuspect>();

        public static List<KeyValuePair<int, TestSuspect>> TestSuspects {

            get {
                return testSuspects.ToList();
            }
        }

        public static void LoadTestSuspects() {


            testSuspects.Add(0, new TestSuspect("C:\\Users\\ratuspro\\Documents\\Virtual Suspect\\Repos\\Virtual-Suspect\\VirtualSuspect\\TestEnvironment\\Resources\\Story\\RobberyStory.xml", "John Doe","Coworker", "Some Text to chill", "C:\\Users\\ratuspro\\Documents\\Virtual Suspect\\Repos\\Virtual-Suspect\\VirtualSuspect\\TestEnvironment\\Resources\\profileDefaultPlaceholder.jpg"));

        }

        public static TestSuspect getTestSuspect(int id) {

            return testSuspects[id];

        }
    }

    public class TestSuspect {

        private List<Question> questions;

        private VirtualSuspectQuestionAnswer virtualSuspect;

        public VirtualSuspectQuestionAnswer VirtualSuspect { get { return virtualSuspect; } }

        private string storyFilePath;

        public string StoryFilePath { get { return storyFilePath; } }

        private string name;

        public string Name { get { return name; } }

        private string connection;

        public string Connection { get { return connection; } }

        private string summary;

        public string Summary { get { return summary; } }

        private string profileImagePath;

        public string ProfileImagePath { get { return profileImagePath; } }

        public TestSuspect(string storyFilePath, string name, string connection, string summary, string profileImagePath) {

            this.storyFilePath = storyFilePath;
            this.name = name;
            this.connection = connection;
            this.summary = summary;
            this.profileImagePath = profileImagePath;

            KnowledgeBaseManager kb = KnowledgeBaseParser.parseFromFile(this.storyFilePath);

            virtualSuspect = new VirtualSuspectQuestionAnswer(kb);

            questions = QuestionManager.LoadQuestions(this.storyFilePath);

        }

        public List<Question> RetrieveAvailableQuestions() {

            //TODO: filter questions

            return questions;
        }
    }
}
