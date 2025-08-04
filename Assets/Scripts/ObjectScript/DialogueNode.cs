[System.Serializable]
public class DialogueNodeJSON
{
    public DialogueData[] dialogues;

    [System.Serializable]
    public class DialogueData
    {
        public string teller;
        public string sentence;
        public string leftSprite;
        public string rightSprite;

        public bool hasChoices;
        public Choice[] choices;
        public string systemName;
        public bool isNarration;
    }

    [System.Serializable]
    public class Choice
    {
        public string choiceText;
        public string nextFile; //json 파일 명 넣기
    }
}