using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtualSuspect.KnowledgeBase;
using VirtualSuspect.Query;
using VirtualSuspectNaturalLanguage.Component;

namespace VirtualSuspectNaturalLanguage
{
    public static class NaturalLanguageGenerator{

        /// <summary>
        /// The method that uses the result to generate an natural language text
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string GenerateAnswer(QueryResult result) {

            string answer = "";
            
            if (result.Query.QueryType == QueryDto.QueryTypeEnum.YesOrNo) { //Yes or no Question

                answer = (result.YesNoResult) ? "Yes" : "No";
            
            } else { //Get Information Question (We assume that the answer only has 1 type of dimension of answer)

                //=============================
                //Get all the answers by dimension
                Dictionary<KnowledgeBaseManager.DimentionsEnum, List<QueryResult.Result>> resultsByDimension = new Dictionary<KnowledgeBaseManager.DimentionsEnum, List<QueryResult.Result>>();
                foreach(QueryResult.Result queryResult in result.Results) {

                    if (!resultsByDimension.ContainsKey(queryResult.dimension)) {
                        resultsByDimension[queryResult.dimension] = new List<QueryResult.Result>();
                    }
                    resultsByDimension[queryResult.dimension].Add(queryResult);
                }

                //==============================
                //Is there any entity with dimension Time
                if (resultsByDimension.ContainsKey(KnowledgeBaseManager.DimentionsEnum.Time)) {

                    bool hasAction;

                    answer += GenerateAnswerBegin(result, KnowledgeBaseManager.DimentionsEnum.Time, out hasAction);

                    //Ignore Cardinality(the same time period only appears once)

                    //Get all the TimeDate and Parse them
                    List<KeyValuePair<DateTime, DateTime>> dateTimeList = new List<KeyValuePair<DateTime, DateTime>>();

                    foreach (QueryResult.Result value in resultsByDimension[KnowledgeBaseManager.DimentionsEnum.Time]) {

                        DateTime firstDate = DateTime.ParseExact(value.values.ElementAt(0).Value.Split('>')[0], "dd/MM/yyyyTHH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime secondDate = DateTime.ParseExact(value.values.ElementAt(0).Value.Split('>')[1], "dd/MM/yyyyTHH:mm:ss", CultureInfo.InvariantCulture);

                        dateTimeList.Add(new KeyValuePair<DateTime, DateTime>(firstDate, secondDate));

                    }

                    //Merge sequence
                    dateTimeList = SortAndMergeSequenceDateTime(dateTimeList);

                    //Group by Day
                    Dictionary<DateTime, List<KeyValuePair<DateTime,DateTime>>> dateTimeGroupByDay = GroupDateTimeByDay(dateTimeList);

                    answer += TimeNaturalLanguageGenerator.Generate(dateTimeGroupByDay);
                    
                }else if(resultsByDimension.ContainsKey(KnowledgeBaseManager.DimentionsEnum.Location)) {

                    bool hasAction;

                    answer += GenerateAnswerBegin(result, KnowledgeBaseManager.DimentionsEnum.Location, out hasAction);

                    answer += LocationNaturalLanguageGenerator.Generate(result, resultsByDimension);
                                      
                }else if (resultsByDimension.ContainsKey(KnowledgeBaseManager.DimentionsEnum.Agent)) {

                    bool hasAction;

                    answer += GenerateAnswerBegin(result, KnowledgeBaseManager.DimentionsEnum.Agent, out hasAction);

                    answer += AgentNaturalLanguageGenerator.Generate(result, resultsByDimension);
                }
                else if (resultsByDimension.ContainsKey(KnowledgeBaseManager.DimentionsEnum.Theme)) {

                    bool hasAction;

                    answer += GenerateAnswerBegin(result, KnowledgeBaseManager.DimentionsEnum.Theme, out hasAction);

                    answer += ThemeNaturalLanguageGenerator.Generate(result, resultsByDimension);
                }
                else if (resultsByDimension.ContainsKey(KnowledgeBaseManager.DimentionsEnum.Manner)) {

                    bool hasAction;

                    answer += GenerateAnswerBegin(result, KnowledgeBaseManager.DimentionsEnum.Manner, out hasAction);

                    answer += MannerNaturalLanguageGenerator.Generate(result, resultsByDimension);
                }
                else if (resultsByDimension.ContainsKey(KnowledgeBaseManager.DimentionsEnum.Reason)) {

                    bool hasAction;

                    answer += GenerateAnswerBegin(result, KnowledgeBaseManager.DimentionsEnum.Reason, out hasAction);

                    if( !hasAction )
                        answer += "to";

                    answer += ReasonNaturalLanguageGenerator.Generate(result, resultsByDimension);

                }

            }

            //Capitalize the answer if needed
            answer = UppercaseFirst(answer);

            //Remove double whitespace
            answer = RemoveWhiteSpace(answer);
            return answer;

        }

        private static string RemoveWhiteSpace(string s) {

            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            return regex.Replace(s, " ");

        }

        private static string UppercaseFirst(string s) {
            // Check for empty string.
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private static bool NewSentencePossible(string sentence) {

            return sentence == "" || sentence.TrimEnd().Last() == '.';

        }
     
        private static List<KeyValuePair<DateTime, DateTime>> SortAndMergeSequenceDateTime(List<KeyValuePair<DateTime, DateTime>> sequence) {

            List<KeyValuePair<DateTime,DateTime>> sequenceMerged = new List<KeyValuePair<DateTime, DateTime>>();

            sequence.Sort((a, b) => a.Key.CompareTo(b.Value));
            
            for(int i = 0; i < sequence.Count; i++) {

                if(i == 0) {
                    sequenceMerged.Add(sequence.ElementAt(0));
                }else {

                    if(sequenceMerged.Last().Value == sequence[i].Key) { //the end of the last is equal to the begin of current Date
                        DateTime beginInterval = sequenceMerged.Last().Key;
                        sequenceMerged.RemoveAt(sequenceMerged.Count - 1);
                        sequenceMerged.Add(new KeyValuePair<DateTime, DateTime>(beginInterval, sequence[i].Value));
                    }else{
                        sequenceMerged.Add(sequence.ElementAt(i));
                    }

                }
                    
            }
                
            return sequenceMerged;    

        }

        private static Dictionary<DateTime, List<KeyValuePair<DateTime,DateTime>>> GroupDateTimeByDay(List<KeyValuePair<DateTime, DateTime>> dateTimeList) {
        
            dateTimeList.Sort((a, b) => a.Key.CompareTo(b.Value));

            IEnumerable<IGrouping<DateTime, KeyValuePair<DateTime, DateTime>>> groupedResult = dateTimeList.GroupBy(x => new DateTime(x.Key.Year, x.Key.Month, x.Key.Day), x => x);

            return groupedResult.ToDictionary(x=>x.Key, x=>x.ToList());
        }

        private static int GetNumberAgents(QueryDto query) {

            return query.QueryConditions.Count(x => x.GetSemanticRole() == KnowledgeBaseManager.DimentionsEnum.Agent);
        }
    
        /// <summary>
        /// Creates the begin for a answer according to the question asked and the agent's number
        /// </summary>
        /// <returns>begin of the answer</returns>
        private static string GenerateAnswerBegin(QueryResult result, KnowledgeBaseManager.DimentionsEnum dimension, out bool hasAction) {

            string answerBegin = "";

            //Create the subject
            int numberAgents = GetNumberAgents(result.Query);

            answerBegin += (numberAgents == 1) ? "I " : "we ";

            //Add verb
            if (result.Query.QueryConditions.Count(x=>x.GetSemanticRole() == KnowledgeBaseManager.DimentionsEnum.Action) == 0) {
                hasAction = false;

                //No action is asked about. Use verb to be
                answerBegin += (numberAgents == 1) ? "was " : "were ";

            } else {
                hasAction = true;

                //Add verb from action resource
                NaturalLanguageResourceManager manager = NaturalLanguageResourceManager.Instance;
                ActionResource resource = manager.FindResource<ActionResource>(result.Query.QueryConditions.Find(x => x.GetSemanticRole() == KnowledgeBaseManager.DimentionsEnum.Action).GetValues().ElementAt(0));
                
                answerBegin += resource.Speech + " ";

                //Add preposition for the dimension
                answerBegin += resource.ExtractPreposition(dimension);

            }

            //Add answer conjunction

            return answerBegin;
            
        }    
        
    }   
}
