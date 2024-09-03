using Azure.Identity;
using Mod.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ModWebsite.Areas.Home.Controllers
{
    [Area("Home")]
    public class ErrorController : Controller
    {
        private readonly ErrorVisual[] visuals = new ErrorVisual[]
        {
            new ErrorVisual("When you try your best but you don't succeed!", "tried your best", 0),//0
            new ErrorVisual("Next time eat a salad", "salad", 0),
            new ErrorVisual("LMG MOUNTED AND LOADED", "tachanka quote", 0),
            new ErrorVisual("https://i.imgur.com/VUpepdn.gif", "Dear God... [image]", 1),
            new ErrorVisual("\"Son, always remember, dying is gay\"", "inspirational advice", 0),
            new ErrorVisual("https://media.tenor.com/va6N4HoJ25MAAAAC/tf2heavy-cry.gif", "Heavy has the big sad [image]", 1),//5
            new ErrorVisual("I never really was on your side.", "not on your side",0),
            new ErrorVisual("Pain, without love. Pain, can't get enough", "such pain", 0),
            new ErrorVisual("https://media.tenor.com/4rCIe_5snNIAAAAC/smg4-mario.gif","here comes the big sad [image]", 1),
            new ErrorVisual("https://media.tenor.com/0PwdZCnWFfoAAAAC/bello-hi.gif","please slap to reboot [image]", 1),
            new ErrorVisual("https://media.tenor.com/XmIq00VuiwoAAAAC/bestmovi-monty.gif","Go away or I shall taunt you a second time [image]", 1),//10
            new ErrorVisual("https://media.tenor.com/QwD9X6Qp-x0AAAAd/tf2-tf2solider.gif","Dramatic death [image]", 1),
            new ErrorVisual("I liked you a lot better when you weren't here", "better without you",0),
            new ErrorVisual("You try me again and i'll hurt you", "don't try me again",0),
            new ErrorVisual(new string[] { "<p class='text-danger h3'>☒ ☒ <br> \\_/ </p>"}, "coob face :D",2),
            new ErrorVisual(new string[] {"This is a 404 screen...", "Dear god...", "there's no more", "No..." }, "this... is a bucket",0),//15
            new ErrorVisual("I'm going to kill you... and then kill you again", "eggman quote",0),
            new ErrorVisual("When at first you don't succeed, give up on your hopes and dreams... wait", "poor advice",0),
            new ErrorVisual("Steak is the superior meat", "best meat",0),
            new ErrorVisual("So anyway I started blasting", "guns ablazing",0),
            new ErrorVisual("If only you knew the things your missing out on", "missing out",0),//20
            new ErrorVisual("This is fine. I'm ok with the events that are unfolding currently", "this is fine",0),
            new ErrorVisual("I have your IP address! It's (REDACTED)", "doxxing time ;)",0),
            new ErrorVisual("According to a friend of mine, cows WILL rule earth some day...", "Rulers of the world",0),
            new ErrorVisual("The amount of time wasted on a 404 screen most will never see... worth it", "Cade contemplating his life choices",0),
            new ErrorVisual("https://media.tenor.com/3_Nl7oUppEQAAAAC/tf2-hummerback.gif","I've done nothing but this for 3 days [image]", 1),//25
            new ErrorVisual("https://media.tenor.com/RJcBOUHIL3cAAAAC/internecion-cube-ic0n.gif","affection given [image]", 1),
            new ErrorVisual("THE HEAVY IS DEAD?", "It is good day to be not ded",0),
            new ErrorVisual("Pizzaaaaaaaaaaaa...", "Chica quote",0),
            new ErrorVisual("Pain is weakness leaving the body", "PAIN is",0),
            new ErrorVisual("https://media.tenor.com/DcNGiTIfv24AAAAd/day-z-fall.gif", "fall through roof",1),//30
            new ErrorVisual("https://media.tenor.com/bwLACNHcMgQAAAAd/spinning-shootingstar.gif", "gokart SpiIiiIIiIIIn [image]",1),
            new ErrorVisual(new string[] {"Never gonna give you up","Never gonna let you down","Never gonna run around and desert you"}, "Ricky sure knows how to roll",0),
            new ErrorVisual("Maybe if you concentrated REALLY HARD and reloaded the page it'll work this time", "MIND PoWeRs",0),
            new ErrorVisual("Mamma mia!", "*confused mario noise*",0),
            new ErrorVisual("\"Tell the world I had a sandwich this evening... extra mayo...\"", "Matt on death row",0),//35
            new ErrorVisual(new string[] {"In a trail of fire I know that we'll be free again", "In the end we will be one", "In a  trail of fire I'll burn before you bury me", "Set your sights for the sun" }, "propane nightmare!",0),
            new ErrorVisual("https://media.tenor.com/owazfIDvuVIAAAAC/tv-smash.gif", "this tv sucks on ice!",1),
            new ErrorVisual("L + Ratio", "Twitter is HELL",0),
            new ErrorVisual("'Tis but a flesh wound", "No it isn't! Your whole bloody arm's off", 0),
            new ErrorVisual("I'm ready, I'm ready", "no your not ready", 0),//40
            new ErrorVisual("I'm gonna BURN your house down! With the lemons!", "angry lemonaid man", 0),
            new ErrorVisual("You did something wrong... existing", "opposite of affection: wishing your death", 0),
            new ErrorVisual("https://media.tenor.com/WXsbu5vppMUAAAAC/unbelievable-carbonfin-gaming.gif", "unbelievable [image]",1),
            new ErrorVisual("https://media.tenor.com/UyVG177SNRYAAAAC/i-cant-believe-youve-done-this.gif", "i can't believe you done this [image]",1),
            new ErrorVisual("https://media.tenor.com/x_OhXRGUjrAAAAAC/star-wars-stormtrooper.gif", "I cant believe you did that! [image]",1),//45
            new ErrorVisual("https://media.tenor.com/9cDZkScHtz0AAAAC/smg4-mario.gif", "mario SPIiIiIiIIIiIiIN [image]",1),
            new ErrorVisual("https://media.tenor.com/huTa7ca0LjkAAAAC/smg4-mario.gif", "mario petrified [image]",1),
            new ErrorVisual(new string[] { "\\|____", "  //\\/\\\\", "Get stickbugged lol"}, "Stick it to ya [image]",0),
            new ErrorVisual(new string[] {"Standing here","I realize","That I regret making a reference","to something I never will watch"}, "stupid quote from something I wont watch", 0),
            new ErrorVisual("I've had it with this FREAKING bad http responses on this FREAKING plane", "metalGEAR! metal gear! metalgear!", 0),//50
            new ErrorVisual("Sure, I love doing anything!", "anything?",0),
            new ErrorVisual("\"Anime was a mistake\" -smart person", "very very smart quote", 0),
            new ErrorVisual("FBI should be knocking on this door any minute now...", "FBI OPEN UP", 0),
            new ErrorVisual("Gonna kill you and I'll keep killin' you and I'll never, cause you're 'onna be dead and then I'm gonna kill you.", "drunken scottish rambling", 0),
            new ErrorVisual("When in doubt, vote Stan out", "Stan was not the imposter", 0),//55
            new ErrorVisual("My disappointment is immeasurable and my day is ruined", "dissaPOINTED", 0),
            new ErrorVisual("https://media.tenor.com/9MFW8t6iRT0AAAAC/parks-and-rec-ron-swanson.gif", "External screaming [image]",1),
            new ErrorVisual("https://media.tenor.com/mrqzEKuvXaMAAAAC/smg4-mario.gif", "No Http? (but spaghetti) [image]",1),
            new ErrorVisual("https://media.tenor.com/YoZYoVqhyNkAAAAC/mario-smg4.gif", "yeah dat makes sense [image]",1),
            new ErrorVisual("https://media.tenor.com/vL3k4DdAPisAAAAC/bob-the-builder-fix-it.gif", "we can't fix it [image]",1),//60
            new ErrorVisual(new string[] {"\"If God had wanted me to live","He would not have created a heart attack\"","-TF2 Soldier"}, "god wishes you ded", 0),
            new ErrorVisual("https://media.tenor.com/X2O4I7lp7lkAAAAC/garfield-cat.gif", "he can DANCE BABY!",1),
            new ErrorVisual("https://media.tenor.com/f3UQahTBeBgAAAAi/demoman-tf2.gif","demoman laugh",1),
            new ErrorVisual("https://media.tenor.com/g20ktKXL20UAAAAi/eggineer.gif","engineer dance",1),
            new ErrorVisual("https://media.tenor.com/p08sU3AwxXMAAAAC/tf2.gif","WAT",1),
            new ErrorVisual("https://media.tenor.com/uDHqrUqjSJ4AAAAd/murder-drones.gif","too sus",1),
            new ErrorVisual(new string[] {"Darn", "Damnit!", "Damnit, fellas!", "Damnit, damnit, damnit, damnit!", "Dagnabit, damnit!", "Damn, dagnabit!", "Dagit nagit, nabit dagit!"},"Engineer Angy",0),
            new ErrorVisual("YOU IMBICILE!!! YOU'VE DOOMED US ALL!","Spy disapproves",0)
        };
        [HttpGet("Error/{id}")]
        [HttpGet("Error")]
        public IActionResult Index(int? id, int? response)
        {
            var vm = new ErrorVM();
            if (id == null)
            {
                vm.code = -1;
            }
            else
            {
                vm.code = (int)id;
            }
            if (response == null || response >= visuals.Length || response < 0) //Let them choose the response
            {
                var ran = new Random();
                vm.visual = visuals[ran.Next(visuals.Length)];
            }
            else
            {
                vm.visual = visuals[(int)response];
            }
            return View(vm);
        }
    }
}
