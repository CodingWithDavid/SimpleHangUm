
namespace HangUm.Pages
{
    public partial class Index
    {
        protected ElementReference myDiv;  // set by the @ref attribute

        private string guess = "";
        private string badGuess = "";
        private string word = "";
        private string lostStyle = "figure-part";
        private int figureParts = 0;
        private string finalWord = "";
        private string notificationState = "";
        private string poplostStyle = "";
        private string popMessage = "Unfortunately you lost. 😕";

        private HangUm.Services.WordService service = new Services.WordService();

        private TimeSpan ts = new TimeSpan(0, 0, 3);

        protected override Task OnInitializedAsync()
        {
            SetWord();
            return base.OnInitializedAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("SetFocusToElement", myDiv);
            }
        }

        protected void KeyDown(KeyboardEventArgs e)
        {
            string temp = e.Key;
            //check to make sure valid key
            if (temp.Length == 1 && char.IsLetter(temp[0]))
            {
                if (figureParts < 6)
                {
                    //is char in word
                    if (word.Contains(temp, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var chars = guess.ToCharArray();
                        for (int i = 0; i < word.Length; i++)
                        {
                            if (temp[0] == word[i])
                            {
                                chars[i] = temp[0];
                            }
                        }
                        //set word back
                        guess = new string(chars);
                    }
                    else
                    {
                        //already guesses?
                        if (!badGuess.Contains(temp, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(badGuess))
                            {
                                badGuess = $"{e.Key}";
                                figureParts++;
                            }
                            else
                            {
                                badGuess = $"{badGuess},{e.Key}";
                                figureParts++;
                            }
                        }
                        else
                        {
                            //show message
                            notificationState = "show";
                            Timmer().Wait();
                        }
                    }
                }

                //last try
                if (guess == word)
                {
                    //game over
                    lostStyle = "popup-show";
                    poplostStyle = "pop-show";
                    finalWord = word;
                    popMessage = "Congratulations! You won! 😃";
                }
                else if (figureParts == 6)
                {
                    lostStyle = "popup-show";
                    poplostStyle = "pop-show";
                    finalWord = word;
                    popMessage = "Unfortunately you lost. 😕";
                }
            }

            if (temp.ToLower() == "enter" && figureParts == 6)
            {
                GameReset().Wait();
            }
            this.StateHasChanged();
        }

        private string SetBodyState(int index)
        {
            string result = "figure-part";

            if (figureParts >= index)
            {
                result = "";
            }
            return result;
        }

        private async Task GameReset()
        {
            figureParts = 0;
            lostStyle = "figure-part";
            badGuess = "";
            poplostStyle = "";
            SetWord();
            await JSRuntime.InvokeVoidAsync("SetFocusToElement", myDiv);
            this.StateHasChanged();
        }

        private async Task Timmer()
        {
            while (ts > new TimeSpan())
            {
                await Task.Delay(1000);
                ts = ts.Subtract(new TimeSpan(0, 0, 1));
            }
            await TimerElapsedHandler();
            StateHasChanged();
        }
        private Task TimerElapsedHandler()
        {
            notificationState = "";
            ts = new TimeSpan(0, 0, 3);
            return Task.CompletedTask;
        }

        private void SetWord()
        {
            word = service.Get().Result;
            guess = "";
            //set guess
            foreach (var c in word)
            {
                guess = guess + " ";
            }
        }
    }
}
