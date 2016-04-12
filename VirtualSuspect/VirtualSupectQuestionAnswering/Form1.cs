﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using VirtualSuspect;
using VirtualSuspect.Query;
using VirtualSuspect.Utils;

namespace VirtualSupectQuestionAnswering
{
    public partial class QuestionAnswerForm : Form {

        KnowledgeBase virtualSuspectKb;

        public QuestionAnswerForm() {
            
            InitializeComponent();

            PopulateQuestionTemplate();

            
        }

        private void btLoadStory_Click(object sender, EventArgs e) {

            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "XML Files(.xml)|*.xml|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {

                lFilePath.Text = openFileDialog1.FileName;
                
                virtualSuspectKb = KnowledgeBaseParser.parseFromFile(openFileDialog1.FileName);

                lStoryStatus.Text = "Story Loaded";    
            }

            gpAnswer.Enabled = true;
            gpQuestion.Enabled = true;
            btViewData.Enabled = true;

        }

        private void btAskQuestion_Click(object sender, EventArgs e) {

            XmlDocument questionXml = new XmlDocument();
            questionXml.LoadXml(tbQuestionStructure.Text);

            QueryDto newQuery = QuestionParser.ExtractFromXml(questionXml);

            QueryResult result = virtualSuspectKb.Query(newQuery);

            tbAnswerStructure.Text = beautify(AnswerGenerator.GenerateAnswer(result));
        }

        private string beautify(XmlDocument doc) {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings)) {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        private void PopulateQuestionTemplate() {

            QuestionTemplate newTemplate = new QuestionTemplate(
                "Where were you on the afternoon of March 1st?",
                @"<question>
    <type>get-information</type>
    <focus>
        <dimension>location</dimension>
    </focus> 
    <condition>
        <dimension>time</dimension>
        <operator>between</operator>
        <begin>01/03/2016T12:00:00</begin>
        <end>01/03/2016T18:59:59</end>
    </condition>
</question>"
            );

            cbQuestionTemplate.Items.Add(newTemplate);

            newTemplate = new QuestionTemplate(
                "What did you access on your computer on March 1st?",
                @"<question>
    <type>get-information</type>
    <focus>
        <dimension>theme</dimension>
    </focus> 
    <condition>
        <dimension>action</dimension>
        <operator>equal</operator>
        <value>Access</value>
    </condition>
    <condition>
        <dimension>manner</dimension>
        <operator>equal</operator>
        <value>Computador do João</value>
    </condition>
    <condition>
        <dimension>time</dimension>
        <operator>between</operator>
        <begin>01/03/2016T00:00:00</begin>
        <end>01/03/2016T23:59:59</end>
    </condition>
</question>" );

            cbQuestionTemplate.Items.Add(newTemplate);

            newTemplate = new QuestionTemplate(
                "Did you talk to Guilherme on March 1st?",
                @"<question>
    <type>yes-no</type>
    <condition>
        <dimension>action</dimension>
        <operator>equal</operator>
        <value>Talk</value>
    </condition>
    <condition>
        <dimension>agent</dimension>
        <operator>equal</operator>
        <value>Guilherme</value>
    </condition>
    <condition>
        <dimension>time</dimension>
        <operator>between</operator>
        <begin>01/03/2016T00:00:00</begin>
        <end>01/03/2016T23:59:59</end>
    </condition>
</question>");

            cbQuestionTemplate.Items.Add(newTemplate);

        }

        private void cbQuestionTemplate_SelectedIndexChanged(object sender, EventArgs e) {

            tbQuestionStructure.Text = ((QuestionTemplate)cbQuestionTemplate.SelectedItem).TemplateContent;
        }

        private void button1_Click(object sender, EventArgs e) {

            DataVisualizer dv = new DataVisualizer(virtualSuspectKb);
            dv.Show();
        }
    }
}
