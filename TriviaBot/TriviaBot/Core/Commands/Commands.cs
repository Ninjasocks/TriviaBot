using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TriviaBot.Core.Commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        //private DiscordSocketClient Client;
        public String[] userArray = new String[10];
        public int[] pointCounter = new int[10];
        public bool[] hasAnswered = new bool[10];

        //command 1
        [Command("Hello"), Alias("helloworld", "world"), Summary("Hello World Command")]
        public async Task Response()
        {
            await Context.Channel.SendMessageAsync("Hello World!");
        }

        [Command("rng"), Alias("rngesus"), Summary("test command")]

        //command 2 - random messages
        public async Task Response2(String msg = null)
        {
            String[] messageArray = { "message1", "message2", "message3", "message4", "message5", "message6", "message7", "message8", "message9" };

            if (msg != null)
            {
                int i = Int32.Parse(msg);

                if (i < messageArray.Length)
                {
                    await Context.Channel.SendMessageAsync(messageArray[i]);
                }

                else
                {
                    Random rand = new Random();
                    int pos = rand.Next(0, messageArray.Length);
                    await Context.Channel.SendMessageAsync("No message at that position!");
                }

            }

        }

        //command 3 - embeds
        [Command("embed"), Summary("Embed Test Command")]
        public async Task Embed([Remainder]string Input = "None")
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Test Embed", Context.User.GetAvatarUrl());
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter("Embed Footer", Context.User.GetAvatarUrl());
            Embed.WithDescription("Description");
            Embed.AddField("User Input: ", Input);

            var msg = await Context.Channel.SendMessageAsync("", false, Embed.Build());

            //testing emoji reactions
            /*
            CancellationTokenSource source = new CancellationTokenSource();

            Emoji emoji = new Emoji("🥚");
            await msg.AddReactionAsync(emoji);
            await Task.Delay(3000, source.Token);
            var users = await msg.GetReactionUsersAsync(emoji, 10).FlattenAsync();
            IUser[] userArray = new IUser[10];
            int[] pointCounter = new int[10];
            int i = 0;

            foreach(IUser user in users)
            {
                userArray[i] = user;
                pointCounter[i] = 0;
                Console.WriteLine(userArray[i]);
                i++;
            }*/
        }

        [Command("trivia"), Alias("triv", "t"), Summary("Trivia Game")]

        public async Task Trivia(String topic = null)
        {

            String[] triviaGames = { "games", "anime", "kpop" };
            string questionFile = @"C:\Users\Ninjasocks\Desktop\triviabot\Trivia\" + topic + ".q";
            string answerFile = @"C:\Users\Ninjasocks\Desktop\triviabot\Trivia\" + topic + ".a";
            CancellationTokenSource source = new CancellationTokenSource();
            EmbedBuilder embed = new EmbedBuilder();
            EmbedBuilder embed2 = new EmbedBuilder();

            if (topic == null)
            {
                embed.WithAuthor("Topics List", null);
                String temp = "";

                for (int i = 0; i < triviaGames.Length; i++)
                {
                    temp += (i + 1) + ". " + triviaGames[i] + "\n";
                }

                embed.WithDescription("Please choose one of the following topics: " + "\n" + temp);

                await Context.Channel.SendMessageAsync("Here's a list of topics!");
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }

            /*
            if (topic.ToLower() == "test")
            {
                answerFile = @"C:\Users\Ninjasocks\Desktop\triviabot\Trivia\games.a";
                String[] answers = File.ReadAllLines(answerFile);
                String answerList = "";

                for (int i = 0; i < answers.Length; i++)
                {
                    answerList += answers[i].ToLower() + "\n";
                }

                await Context.Channel.SendMessageAsync(answerList);
            }
            */

            /*
            if (topic.ToLower() == "random" || topic.ToLower() == "rng")
            {
                Random rand = new Random();
                int randTopicNum = rand.Next(0, triviaGames.Length);
                await Context.Channel.SendMessageAsync("Random topic chosen! The topic is: " + triviaGames[randTopicNum]);
                await JoinTrivia();
            }*/

            if (triviaGames.Any(topic.ToLower().Contains))
            {
                String[] questions = File.ReadAllLines(questionFile);
                String[] answers = File.ReadAllLines(answerFile);
                String[] answeredQuestions = new String[questions.Length];

                await JoinTrivia();

                if (userArray[0] == null) //no players joined
                {
                    await Context.Channel.SendMessageAsync("Not enough players!");
                }

                else
                {
                    Random rand = new Random();
                    int pos = rand.Next(0, questions.Length);
                    answeredQuestions[pos] = questions[pos];

                    bool stopGame = false;

                    for (int i = 0; i < questions.Length; i++)
                    {
                        if (i >= questions.Length)
                        {
                            stopGame = true;
                            break;
                        }

                        else if (stopGame)
                        {
                            break;
                        }

                        else {; }

                        embed.WithTitle("Question " + (i + 1));
                        embed.WithDescription(questions[pos]);
                        embed.WithFooter("Question " + (i + 1) + " of " + questions.Length);

                        var questionMessage = await Context.Channel.SendMessageAsync("", false, embed.Build());
                        await Task.Delay(10000, source.Token);
                        var nextMessages = await Context.Channel.GetMessagesAsync(questionMessage.Id, Direction.After, 100).FlattenAsync();

                        foreach (var guess in nextMessages)
                        {
                            int index = 0; //reset index

                            Console.WriteLine(($"{guess.Author} posted '{guess.Content}' at {guess.CreatedAt}."));

                            if (guess.Content.ToLower() == "triviastop")
                            {
                                stopGame = true;
                                break;
                            }

                            else if (guess.Content.ToLower().Equals(answers[pos].ToLower()) && userArray.Contains(guess.Author.Username))
                            {
                                Console.WriteLine(($"{guess.Author} guessed correctly"));

                                for (int j = 0; j < userArray.Length; j++) //find where they are in userArray
                                {
                                    if (userArray[j] != guess.Author.Username)
                                    {
                                        index++;
                                    }

                                    else
                                    {
                                        Console.WriteLine(index);
                                        //index = 0;
                                        break;
                                    }
                                }

                                if (!hasAnswered[index]) //if user at index has not provided an answer yet and is correct, increase their score 
                                {
                                    pointCounter[index]++; //give a point
                                    hasAnswered[index] = true; //prevents them from gaining multiple points by spamming the correct answer during a question
                                }

                                Console.WriteLine(pointCounter[index]);
                                Console.WriteLine(hasAnswered[index]);
                            }

                            else
                            {
                                ; //do nothing
                            }
                        }

                        String correctAnswers = "";

                        for (int k = 0; k < pointCounter.Length; k++)
                        {
                            if (hasAnswered[k])
                            {
                                correctAnswers += userArray[k] + "\n";
                            }
                        }

                        embed2.WithTitle("The correct answer is " + answers[pos]);
                        embed2.WithDescription("People who guessed correctly: \n" + correctAnswers);
                        if((i+1) == questions.Length)
                        {
                            embed2.WithFooter("Final Question!");
                        }

                        else
                        {
                            embed2.WithFooter("Next Question!");
                        }

                        await Context.Channel.SendMessageAsync("", false, embed2.Build());

                        for (int l = 0; l < hasAnswered.Length; l++)
                        {
                            hasAnswered[l] = false;
                        }

                        while (answeredQuestions[pos] == questions[pos]) //prevent question repetition
                        {
                            pos = rand.Next(0, questions.Length);
                        }

                        answeredQuestions[pos] = questions[pos];
                    }


                    await Context.Channel.SendMessageAsync("Game is now over! The scores are the following: ");
                    String results = "";
                    for (int i = 0; i < userArray.Length; i++)
                    {
                        if (userArray[i] != null)
                        {
                            results += userArray[i] + ": " + pointCounter[i] + " points \n";
                        }

                    }

                    await Context.Channel.SendMessageAsync(results);

                    //await Context.Channel.SendMessageAsync(File.Exists(questionFile) + "");
                    //await Context.Channel.SendMessageAsync("Valid Topic!");
                }
            }

            else
            {
                await Context.Channel.SendMessageAsync("No such trivia topic!");
            }

        }

        public async Task JoinTrivia()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            Emoji emoji = new Emoji("🥚");
            var msg = await Context.Channel.SendMessageAsync("React to this message in the next 10 seconds if you would like to join trivia!");
            await msg.AddReactionAsync(emoji);
            await Task.Delay(10000, source.Token);
            var users = await msg.GetReactionUsersAsync(emoji, 10).FlattenAsync();
            int i = 0;

            foreach (IUser user in users)
            {
                if (!user.IsBot)
                {
                    userArray[i] = user.Username;
                    pointCounter[i] = 0;
                    hasAnswered[i] = false;
                    Console.WriteLine(userArray[i] + " joined trivia");
                    i++;
                }

            }

            await Context.Channel.SendMessageAsync("Trivia will begin in 10 seconds! You will have 10 seconds to guess the answer for each question. Get ready!");
            //await Task.Delay(3000, source.Token);
            await Task.Delay(10000, source.Token);
        }

        [Command("mal"), Alias("MAL", "myanimelist", "search"), Summary("MyAnimeList search")]

        public async Task MALSearch([Remainder]String name = null)
        {
            if (name == null)
            {
                await Context.Channel.SendMessageAsync("Please have a search term!");
            }

            else
            {
                if (name.Split().Length > 1) //search string longer than 1 word
                {
                    String[] words = name.Split(' ');
                    String temp = "";

                    for (int i = 0; i < words.Length; i++)
                    {
                        temp += words[i] + "%20";
                    }

                    await Context.Channel.SendMessageAsync("MAL Search: \n https://myanimelist.net/search/all?q=" + temp);
                }

                else
                {
                    await Context.Channel.SendMessageAsync("MAL Search: \n https://myanimelist.net/search/all?q=" + name);
                }
            }
        }

        [Command("fox"), Alias("foxes", "Fox", "Foxes", "kitsune", "senko"), Summary("Fox image generator")]

        public async Task FoxImage()
        {
            String[] imageArray = Directory.GetFiles(@"C:\Users\Ninjasocks\Desktop\triviabot\Images\Fox\"); //get all file names + directory in fox folder
            Random rand = new Random();
            int num = rand.Next(0, imageArray.Length); //generate random number between 0 and total number of images in the Fox folder
            await Context.Channel.SendFileAsync(imageArray[num], "Have a picture of a fox!");
        }

        [Command("panda"), Alias("PANDA", "Panda"), Summary("Panda image generator")]

        public async Task PandaImage()
        {
            String[] imageArray = Directory.GetFiles(@"C:\Users\Ninjasocks\Desktop\triviabot\Images\Panda\"); //get all file names + directory in fox folder
            Random rand = new Random();
            int num = rand.Next(0, imageArray.Length); //generate random number between 0 and total number of images in the Fox folder
            await Context.Channel.SendFileAsync(imageArray[num], "Have a picture of a panda!");
        }

        [Command("penguin"), Alias("Penguin", "pengu", "Pengu", "PENGUIN"), Summary("Penguin image generator")]

        public async Task PenguinImage()
        {
            String[] imageArray = Directory.GetFiles(@"C:\Users\Ninjasocks\Desktop\triviabot\Images\Penguin\"); //get all file names + directory in fox folder
            Random rand = new Random();
            int num = rand.Next(0, imageArray.Length); //generate random number between 0 and total number of images in the Fox folder
            await Context.Channel.SendFileAsync(imageArray[num], "Have a picture of a penguin!");
        }

        [Command("dog"), Alias("dogs", "doggos", "doggies", "Dog", "DOGS", "DOG"), Summary("Dog image generator")]

        public async Task DogImage()
        {
            String[] imageArray = Directory.GetFiles(@"C:\Users\Ninjasocks\Desktop\triviabot\Images\Dog\"); //get all file names + directory in fox folder
            Random rand = new Random();
            int num = rand.Next(0, imageArray.Length); //generate random number between 0 and total number of images in the Fox folder
            await Context.Channel.SendFileAsync(imageArray[num], "Have a picture of a dog!");
        }

        [Command("cat"), Alias("cats", "kitty", "kitten", "KITTY", "CATS", "CAT"), Summary("Cat image generator")]

        public async Task CatImage()
        {
            String[] imageArray = Directory.GetFiles(@"C:\Users\Ninjasocks\Desktop\triviabot\Images\Cat\"); //get all file names + directory in fox folder
            Random rand = new Random();
            int num = rand.Next(0, imageArray.Length); //generate random number between 0 and total number of images in the Fox folder
            await Context.Channel.SendFileAsync(imageArray[num], "Have a picture of a cat!");
        }

        [Command("test"), Summary("test command")]

        public async Task Test()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            await JoinTrivia();

            var test = await Context.Channel.SendMessageAsync("Test");
            await Task.Delay(3000, source.Token); //delays for 3 seconds
            var nextMessages = await Context.Channel.GetMessagesAsync(test.Id, Direction.After, 5).FlattenAsync();

            foreach (var msg in nextMessages)
            {
                Console.WriteLine($"{msg.Author} posted '{msg.Content}' at {msg.CreatedAt}.");
                Console.WriteLine(userArray.Contains(msg.Author.Username));

                int index = 0;
                for (int j = 0; j < userArray.Length; j++) //find where they are in userArray
                {
                    if (userArray[j] != msg.Author.Username)
                    {
                        index++;
                    }

                    else
                    {
                        Console.WriteLine(index);
                        index = 0;
                        break;
                    }
                }
            }
        }
    }
}
