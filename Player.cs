using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace VocalCords
{
    //PLS READ

    //I commented the code to make it as accessible as possible. The code can be improved ALOT I know.
    //Everything should be understandable even to someone with no modding experience who just wants to customize this.
    //Same blocks of codes are not explained twice as they follow whatever rules I said for the block before.
    //If you know how to mod (even basically or simply know C#) and want to modify the systems these comments should be enough
    //You should be also able to modify the systems with no experience by following the comments
    //If you just want to change the quotes or add more (without changing the quote parts systems and such) just scroll until you see a bunch of cases and strings
    //If you know nothing about modding or C# I suggest reading all of the comments as thing are explained only once and only the first time they appear (how to add more quotes is explained from line 104)

    //If you have any problems I'm happy to help. My Discord is clancini or you can just check the tmodloader discord for help.
    internal class Player : ModPlayer
    {

        //Colors of the quotes (text above the player). To add more just Copy and paste, change the name and the rgb value
        Random rnd = new Random();

        Color idle = new Color(255, 255, 255);
        Color sad = new Color(168, 168, 168);
        Color combat = new Color(255, 120, 120);
        Color hype = new Color(255, 217, 112);
        Color hypecmbt = new Color(255, 66, 66);

        Color current = new Color(255, 255, 255); //placeholder for the color variable

        //Variables used in the mod. Change these to change how the hype mechanics and combat quotes behave

        bool highHype = false;  //Does the player have Hype?
        int hypeValue = 0;      //How much hype does the player have
        int hypeToTrigger = 3000;   //How much hype does the player need to show unique quotes?

        int hypeCooldown = 0;   //Current hype timer cooldown (0 = not in cooldown)
        int maxHypeCooldown = 600;  //Just for ease of changing the hype cooldown timer. Used like this -> hypeCooldown = maxHypeCooldown

        bool inCombat = false;  //Is the player in combat?
        int inCombatTimer = 0;  //Current combat timer value (0 = not in combat)
        int inCombatMaxTime = 600;  //Same as before, for ease of changing the combat cooldown timings. Used like this -> inCombatTimer = inCombatMaxTime

        int quoteDelay = 0; //How much time between quote parts
        int quoteCD = 0;   //Same as last one just for the normal quotes (such as combat)
        int idleQuoteCD = 0;   //Current quote delay timer value (0 = can say another quote again, >0 player can't talk). Used for idle quotes, which should have higher CD
        bool isIdleQuote = false;   //Is the current quote an idle quote. Just used this for my own process, you can delete it (I think)
        
        //Quote pieces
        //The quotes are formed by a maximum of 3 parts, to allow the quote to be actually readable and not too fast
        //You can add more parts to make longer quotes just add another String below

        String Part1 = "";
        String Part2 = "";
        String Part3 = "";

        public override void PreUpdate(){
            //Timers----------------------------------------------------------
            //All timers basically decrease by 1 each frame (1/60 of a second) if they are above 0

            if(hypeCooldown > 0){       //If the Player recently had hype, they can't have it again
                hypeCooldown -= 1;
                hypeValue = 0;
            }

            if(inCombat && inCombatTimer > 0){      //If the Player is in combat, decrease the timer each frame
                inCombatTimer -= 1;
            }
            if(inCombatTimer <= 0 && inCombat){                 //If the timer is 0, the Player is not in combat
                Part1 = "";
                Part2 = "";
                Part3 = "";
                inCombat = false;
            }
            
            //If the player is near an enemy, gain hype (1 each frame)
            if(FindClosestNPC(500) != null && !FindClosestNPC(500).friendly && !FindClosestNPC(500).boss)
            {
                hypeValue += 1;
            }else if (FindClosestNPC(500) != null && FindClosestNPC(500).boss) //if it's a boss it's 2 per frame
            {
                hypeValue += 2;
            }

            if (!inCombat && hypeValue > 0){         //Hype decrease each frame if not in combat
                hypeValue -= 1;
            }

            if(hypeValue >= hypeToTrigger && !highHype && hypeCooldown == 0){                  //If the Player has enough hype, highHype = true
                Part1 = ""; //reset quote parts and CD to avoid conflicts (trust me keep these)
                Part2 = "";
                Part3 = "";
                quoteCD = 0;
                idleQuoteCD += 60;
                SoundEngine.PlaySound(SoundID.Item68);  //let the player know something happened
                    highHype = true;
                    int quote = rnd.Next(9);    //This gives a random number between 0 and value-1 (included)

                    switch (quote)      //Getting hype quotes, change these as you want
                                        //current refers to the current color of the quote, as defined at the top of the code
                                        //Just add cases (and/or parts if you implemented support for more than 3) as you see fit to add more quotes.
                    {
                        case 0:
                            current = hype;
                            Part1 = "I look good";
                            Part2 = "I feel good";
                            Part3 = "Let's kick some ass!";
                            break;
                        case 1:
                            current = hype;
                            Part1 = "I'll turn this place to DUST!";
                            break;
                        case 2:
                            current = hype;
                            Part1 = "I'll tear this place UP!";
                            break;
                        case 3:
                            current = hype;
                            Part1 = "Hoohoo! I feel a fight comin'.";
                            break;
                        case 4:
                            current = hype;
                            Part1 = "I can't wait to get in there";
                            Part2 = "I don't know what I'm going to do";
                            Part3 = "But I am going to get in there!";
                            break;
                        case 5:
                            current = hype;
                            Part1 = "They think they can catch ME?";
                            break;
                        case 6:
                            current = hype;
                            Part1 = "I don't do safe!";
                            break;
                        case 7:
                            current = hype;
                            Part1 = "I'm gonna live forever!";
                            break;
                        case 8:
                            current = hype;
                            Part1 = "It ain't a party without a fight!";
                            break;
                    }
            }
            else if(hypeValue < hypeToTrigger && highHype){    //Once the hype is decayed enough, stop highHype
                SoundEngine.PlaySound(SoundID.Shatter);
                hypeCooldown = maxHypeCooldown;
                highHype = false;
                Part1 = "";
                Part2 = "";
                Part3 = "";
                quoteCD = 0;
                idleQuoteCD += 60;
                int quote = rnd.Next(9);

                switch (quote)
                {
                    case 0:
                        current = idle;
                        Part1 = "Woo";
                        Part2 = "That was intense!";
                        break;
                    case 1:
                        current = sad;
                        Part1 = "Aww...";
                        break;
                    case 2:
                        current = sad;
                        Part1 = "The party's already over?";
                        Part2 = "Pfft.";
                        break;
                    case 3:
                        current = sad;
                        Part1 = "Party foul.";
                        break;
                    case 4:
                        current = sad;
                        Part1 = "Ah come ON!";
                        break;
                    case 5:
                        current = idle;
                        Part1 = "Ok. I'm rested";
                        Part2 = "Let's get interestng!";
                        break;
                    case 6:
                        current = idle;
                        Part1 = "Gotta keep movin'!";
                        break;
                    case 7:
                        current = sad;
                        Part1 = "Whoops.";
                        break;
                    case 8:
                        current = idle;
                        Part1 = "Yep! I'm amazing!";
                        break;
                }
            }

            //Cooldowns management
            if(quoteDelay > 0){
                quoteDelay -= 1;
            }
            if(quoteCD > 0){
                quoteCD -= 1;
            }
            if(idleQuoteCD > 0){
                idleQuoteCD -= 1;
            }

            //This is what all the mod is based on
            //This can be expanded easily (probably) if you want longer quotes with more parts
   
            if(Part1 != "" && quoteDelay == 0 && quoteCD == 0 && idleQuoteCD < 1800 && !isIdleQuote){       //If the plater can say a quote (you can define when this is the case, just change the if condition. In this case i all his CDs are 0 except for the idleQuote which could have been said 1800 frames ago)
                Main.combatText[CombatText.NewText(Player.Hitbox, current, Part1, false)].lifeTime = 120;   //This lets you customize the quote properties. (you probably want the first parameter as it is, the second parameter is the color, the third the quote part and the forth if you want bold)
                quoteDelay = 120;   //you can change this to modify how much time passes (in frames) between parts
                if(Part2 == ""){    //If the next part is empty, the quote is over, add cooldown as normal
                    quoteCD = 600;
                }else{                 //If not, add quoteDelay so the next part is said X frames later
                    quoteCD = quoteDelay;
                }

                Part1 = ""; //Clear the part that the player just said
            }
            
            if(Part1 == "" && Part2 != "" && quoteCD == 0 && quoteDelay == 0 && !isIdleQuote){
                Main.combatText[CombatText.NewText(Player.Hitbox, current, Part2, false)].lifeTime = 120;
                quoteDelay = 120;
                if(Part3 == ""){
                    quoteCD = 600;
                }else{
                    quoteCD = quoteDelay;
                }

                Part2 = "";
            }
            if(Part1 == "" && Part2 == "" && quoteCD == 0 && Part3 != "" && quoteDelay == 0 && !isIdleQuote){
                Main.combatText[CombatText.NewText(Player.Hitbox, current, Part3, false)].lifeTime = 120;
                quoteDelay = 120;           //This is the maximum quote I support, so I always add the normal CD time
                quoteCD = 600;

                Part3 = "";
            }

            //You can support n quote parts by just doing this \/
            //Just follow the logic of these if()s to add support for more than 3 parts.
            //Copy the 1st or 2nd block adjust the condition by adding Part1 == "" && Part2 == "" && Part3 != "" (to add one part)
            //then change the last if's confition to check if all parts are empty

            //This works the same, just for idlee quotes (which means longer CDs)
            if (Part1 != "" && quoteDelay == 0 && quoteCD == 0 && idleQuoteCD == 0 && isIdleQuote){
                Main.combatText[CombatText.NewText(Player.Hitbox, current, Part1, false)].lifeTime = 120;
                quoteDelay = 120;
                idleQuoteCD = quoteDelay;
                if(Part2 == ""){
                    idleQuoteCD = 3000;
                    isIdleQuote = false;
                }
                Part1 = "";
            }
            if(Part1 == "" && Part2 != "" && quoteCD == 0 && quoteDelay == 0 && idleQuoteCD == 0 && isIdleQuote){
                Main.combatText[CombatText.NewText(Player.Hitbox, current, Part2, false)].lifeTime = 120;
                quoteDelay = 120;
                idleQuoteCD = quoteDelay;
                if(Part3 == ""){
                    idleQuoteCD = 3000;
                    isIdleQuote = false;
                }
                Part2 = "";
            }
            if(Part1 == "" && Part2 == "" && quoteCD == 0 && Part3 != "" && quoteDelay == 0 && idleQuoteCD == 0 && isIdleQuote){
                Main.combatText[CombatText.NewText(Player.Hitbox, current, Part3, false)].lifeTime = 120;
                quoteDelay = 120;
                idleQuoteCD = 3000;
                isIdleQuote = false;
                Part3 = "";
            }

            //Idle quotes no hype
            if(!inCombat && idleQuoteCD == 0 && !highHype){
                Part1 = "";
                Part2 = "";
                Part3 = "";
                int quote = rnd.Next(8);

                switch(quote){
                    case 0:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "Man";
                        Part2 = "Have you seen that forest over there?";
                    break;
                    case 1:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "Crap.";
                        Part2 = "I totally forgot why I'm here.";
                    break;
                    case 2:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "When i get sad";
                        Part2 = "I stop being sad and be awesome instead.";
                        Part3 = "True story.";
                    break;
                    case 3:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "Danger is as fun as you let it be.";
                    break;
                    case 4:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "I'm not blind to any possibility.";
                    break;
                    case 5:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "The world's a big place.";
                        Part2 = "Don't want to miss any of it.";
                    break;
                    case 6:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "It's a revolution.";
                        Part2 = "And it's good times.";
                    break;
                    case 7:
                        current = idle;
                        isIdleQuote = true;
                        Part1 = "Long ago";
                        Part2 = "Our kind was free.";
                        Part3 = "Now only their shadow remains.";
                    break;
                }
            }else if (!inCombat && idleQuoteCD == 0 && highHype)        //idle hype quotes
            {
                Part1 = "";
                Part2 = "";
                Part3 = "";
                int quote = rnd.Next(8);

                switch (quote)
                {
                    case 0:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "Fun";
                        Part2 = "I need some fun";
                        Part3 = "NOW!";
                        break;
                    case 1:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "Can we just get started already?";
                        break;
                    case 2:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "Why are you SO boring?";
                        break;
                    case 3:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "Oh";
                        Part2 = "You're bumming me out!";
                        break;
                    case 4:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "C'mon";
                        Part2 = "Let's dance!";
                        break;
                    case 5:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "We will not fade away!";
                        break;
                    case 6:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "Am I not magnificent?";
                        break;
                    case 7:
                        current = hype;
                        isIdleQuote = true;
                        Part1 = "Nothing's fun without risk!";
                        Part2 = "C'mon";
                        Part3 = "I got some moves for you!";
                        break;
                }
            }

            //Exiting combat no hype
            if (inCombatTimer < 60 && quoteCD == 0 && idleQuoteCD < 1500 && !highHype && inCombat){
                Part1 = "";
                Part2 = "";
                Part3 = "";
                int quote = rnd.Next(5);

                switch(quote){
                    case 0:
                        current = idle;
                        Part1 = "Good job";
                        Part2 = Player.name + "!";
                    break;
                    case 1:
                        current = idle;
                        Part1 = Player.name;
                        Part2 = "Doing his job.";
                    break;
                    case 2:
                        current = idle;
                        Part1 = "One step closer to freedom.";
                    break;
                    case 3:
                        current = idle;
                        Part1 = "I'm hungry.";
                    break;
                    case 4:
                        current = idle;
                        Part1 = "I'm feeling alright!";
                    break;
                }
            }else if (inCombatTimer < 60 && quoteCD == 0 && idleQuoteCD < 1500 && highHype && inCombat) //Exiting while in hype
            {
                Part1 = "";
                Part2 = "";
                Part3 = "";
                int quote = rnd.Next(5);

                switch (quote)
                {
                    case 0:
                        current = hype;
                        Part1 = "I got some more moves for you too!";
                        break;
                    case 1:
                        current = hype;
                        Part1 = "Who";
                        Part2 = "Wants";
                        Part3 = "To dance?";
                        break;
                    case 2:
                        current = hype;
                        Part1 = "Hello!";
                        Part2 = "You ready for me?";
                        break;
                    case 3:
                        current = hype;
                        Part1 = "Ain't gonna slow down!";
                        break;
                    case 4:
                        current = hype;
                        Part1 = "I don't do safe!";
                        break;
                }
            }
            base.PreUpdate();

        }
        
        //Melee quotes
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            hypeValue += 5;
            inCombat = true;
            inCombatTimer = inCombatMaxTime;

            if(quoteCD == 0 && idleQuoteCD < 1500 && !highHype){
                Part1 = "";
                Part2 = "";
                Part3 = "";
                int quote = rnd.Next(8);

                switch(quote){
                    case 0:
                        current = combat;
                        Part1 = "Sho!";
                    break;
                    case 1:
                        current = combat;
                        Part1 = "Slap!";
                    break;
                    case 2:
                        current = combat;
                        Part1 = "How's that face?";
                    break;
                    case 3:
                        current = combat;
                        Part1 = "Get some!";
                    break;
                    case 4:
                        current = combat;
                        Part1 = "Woo!";
                    break;
                    case 5:
                        current = combat;
                        Part1 = "My palm sends its regards!";
                    break;
                    case 6:
                        current = combat;
                        Part1 = "Oh yeah!";
                    break;
                    case 7:
                        current = combat;
                        Part1 = "Man";
                        Part2 = "THAT's insulting!";
                    break;
                }
            }else if (quoteCD == 0 && idleQuoteCD < 1500 && highHype) //hype variant melee
            {
                Part1 = "";
                Part2 = "";
                Part3 = "";
                int quote = rnd.Next(8);

                switch (quote)
                {
                    case 0:
                        current = hypecmbt;
                        Part1 = "YOU're trying to upstage ME!?";
                        break;
                    case 1:
                        current = hypecmbt;
                        Part1 = "Oh! Can't touch this!";
                        break;
                    case 2:
                        current = hypecmbt;
                        Part1 = "Try to hit me fool!";
                        break;
                    case 3:
                        current = hypecmbt;
                        Part1 = "They ain't ready!";
                        Part2 = "They. Ain't. Ready!";
                        break;
                    case 4:
                        current = hypecmbt;
                        Part1 = "Hell, let's do it!";
                        break;
                    case 5:
                        current = hypecmbt;
                        Part1 = "I'm gonna slap all o' you!";
                        break;
                    case 6:
                        current = hypecmbt;
                        Part1 = "Count it down!";
                        Part2 = "One, two...";
                        Part3 = "You're dead!";
                        break;
                    case 7:
                        current = hypecmbt;
                        Part1 = "Feeling insulted yet?";
                        break;
                }
            }

            base.OnHitNPC(item, target, damage, knockback, crit);
        }

        //All attack quotes (projectile based weapons, which includes certain melee weapons)
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            inCombat = true;
            inCombatTimer = inCombatMaxTime;

            if (proj.DamageType == DamageClass.Melee || proj.DamageType == DamageClass.SummonMeleeSpeed || proj.DamageType == DamageClass.MeleeNoSpeed) //melee 
            {
                hypeValue += 5;
                //Hit melee quotes
                if (!highHype && quoteCD == 0)  //no hype melee
                {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";

                    int quote = rnd.Next(8);
                    switch (quote)
                    {
                        case 0:
                            current = combat;
                            Part1 = "Sho!";
                            break;
                        case 1:
                            current = combat;
                            Part1 = "Slap!";
                            break;
                        case 2:
                            current = combat;
                            Part1 = "How's that face?";
                            break;
                        case 3:
                            current = combat;
                            Part1 = "Get some!";
                            break;
                        case 4:
                            current = combat;
                            Part1 = "Woo!";
                            break;
                        case 5:
                            current = combat;
                            Part1 = "My palm sends its regards!";
                            break;
                        case 6:
                            current = combat;
                            Part1 = "Oh yeah!";
                            break;
                        case 7:
                            current = combat;
                            Part1 = "Man";
                            Part2 = "THAT's insulting!";
                            break;
                    }
                }
                else if (quoteCD == 0 && idleQuoteCD < 1500 && highHype)    //hype melee
                {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";
                    int quote = rnd.Next(8);

                    switch (quote)
                    {
                        case 0:
                            current = hypecmbt;
                            Part1 = "YOU're trying to upstage ME!?";
                            break;
                        case 1:
                            current = hypecmbt;
                            Part1 = "Oh! Can't touch this!";
                            break;
                        case 2:
                            current = hypecmbt;
                            Part1 = "Try to hit me fool!";
                            break;
                        case 3:
                            current = hypecmbt;
                            Part1 = "They ain't ready!";
                            Part2 = "They. Ain't. Ready!";
                            break;
                        case 4:
                            current = hypecmbt;
                            Part1 = "Hell, let's do it!";
                            break;
                        case 5:
                            current = hypecmbt;
                            Part1 = "I'm gonna slap all o' you!";
                            break;
                        case 6:
                            current = hypecmbt;
                            Part1 = "Count it down!";
                            Part2 = "One, two...";
                            Part3 = "You're dead!";
                            break;
                        case 7:
                            current = hypecmbt;
                            Part1 = "Feeling insulted yet?";
                            break;
                    }
                }
              
            }
            else if(proj.DamageType == DamageClass.Magic)   //no hype magic
            {
                hypeValue += 3;
                //Hit magic quotes
                if (!highHype && quoteCD == 0)
                {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";
                    int quote = rnd.Next(8);

                    switch(quote){
                        case 0:
                            current = combat;
                            Part1 = "My magic is a fury!";
                        break;
                        case 1:
                            current = combat;
                            Part1 = "My magic is wild!";
                        break;
                        case 2:
                            current = combat;
                            Part1 = "Magic is a wilderness!";
                        break;
                        case 3:
                            current = combat;
                            Part1 = "Magic is supposed to hurt a little!";
                        break;
                        case 4:
                            current = combat;
                            Part1 = "Did you see that?";
                        break;
                        case 5:
                            current = combat;
                            Part1 = "I love fireworks.";
                        break;
                        case 6:
                            current = combat;
                            Part1 = "Lit.";
                        break;
                        case 7:
                            current = combat;
                            Part1 = "I'm only as wild as my magic.";
                        break;
                    }
                }
                else if (quoteCD == 0 && idleQuoteCD < 1500 && highHype)    //magic hype
                {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";
                    int quote = rnd.Next(8);

                    switch (quote)
                    {
                        case 0:
                            current = hypecmbt;
                            Part1 = "I'll burn this place to the ground!";
                            break;
                        case 1:
                            current = hypecmbt;
                            Part1 = "You'll turn into ash!";
                            break;
                        case 2:
                            current = hypecmbt;
                            Part1 = "I don't even know what this thing does!";
                            Part2 = "Oh";
                            Part3 = "It's awesome!";
                            break;
                        case 3:
                            current = hypecmbt;
                            Part1 = "Magic is a dance";
                            Part2 = "And my moves KILL!";
                            break;
                        case 4:
                            current = hypecmbt;
                            Part1 = "Goddamn!";
                            break;
                        case 5:
                            current = hypecmbt;
                            Part1 = "I am born of magic";
                            Part2 = "And you DIE of magic!";
                            break;
                        case 6:
                            current = hypecmbt;
                            Part1 = "Magic is fire!";
                            break;
                        case 7:
                            current = hypecmbt;
                            Part1 = "Pay attention!";
                            Part2 = "And I know";
                            Part3 = "I am very distracting!";
                            break;
                    }
                }
            }
            else if (proj.DamageType == DamageClass.Ranged) //ranged no hype
            {
                hypeValue += 2;
                //Hit ranged quotes
                if (!highHype && quoteCD == 0)
                {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";
                    int quote = rnd.Next(8);

                    switch(quote){
                        case 0:
                            current = combat;
                            Part1 = "Could do this with my eyes closed.";
                        break;
                        case 1:
                            current = combat;
                            Part1 = "Pew!";
                            Part2 = "Pew!";
                            Part3 = "Pow!";
                        break;
                        case 2:
                            current = combat;
                            Part1 = "I look as good as I aim.";
                        break;
                        case 3:
                            current = combat;
                            Part1 = "Come ON!";
                            Part2 = "That was easy to dodge.";
                        break;
                        case 4:
                            current = combat;
                            Part1 = "I like moving targets.";
                        break;
                        case 5:
                            current = combat;
                            Part1 = "Achoo!";
                            Part2 = "Wooh...";
                            Part3 = "Messed up my shot.";
                        break;
                        case 6:
                            current = combat;
                            Part1 = "Hole in one!";
                        break;
                        case 7:
                            current = combat;
                            Part1 = "My hands are magic!";
                        break;
                    }
                }else if (quoteCD == 0 && idleQuoteCD < 1500 && highHype)   //ranged hype
                    {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";
                    int quote = rnd.Next(8);

                    switch (quote)
                    {
                        case 0:
                            current = hypecmbt;
                            Part1 = "You here for an autograph?";
                            Part2 = "Lucky you!";
                            Part3 = "This' the last bullet signed by me!";
                            break;
                        case 1:
                            current = hypecmbt;
                            Part1 = "Trust me";
                            Part2 = "A fist fight would have been worse";
                            Part3 = "For you that is.";
                            break;
                        case 2:
                            current = hypecmbt;
                            Part1 = "One shot...";
                            Part2 = "And everybody's dead!";
                            break;
                        case 3:
                            current = hypecmbt;
                            Part1 = "Magic is a dance";
                            Part2 = "And my moves KILL!";
                            break;
                        case 4:
                            current = hypecmbt;
                            Part1 = "Woohoo!";
                            Part2 = "Keep dancing!";
                            break;
                        case 5:
                            current = hypecmbt;
                            Part1 = "To hell with magic!";
                            Part2 = "Lead works wonders!";
                            break;
                        case 6:
                            current = hypecmbt;
                            Part1 = "Splat!";
                            break;
                        case 7:
                            current = hypecmbt;
                            Part1 = "It's like i can hear the music.";
                            Part2 = "Weird, huh?";
                            break;
                    }
                }
            }
            else if (proj.DamageType == DamageClass.Summon) //summoner no hype
            {
                hypeValue += 2;
                //Hit ranged quotes
                if (!highHype && quoteCD == 0)
                {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";
                    int quote = rnd.Next(8);

                    switch (quote)
                    {
                        case 0:
                            current = combat;
                            Part1 = "No hands!";
                            break;
                        case 1:
                            current = combat;
                            Part1 = "Don't look at me!";
                            Part2 = "It ain't me!";
                            break;
                        case 2:
                            current = combat;
                            Part1 = "No!";
                            Part2 = "I said attack THEM!";
                            break;
                        case 3:
                            current = combat;
                            Part1 = "Lucky I ain't the one touching you.";
                            break;
                        case 4:
                            current = combat;
                            Part1 = "Love watching a good fight.";
                            Part2 = "No not you";
                            Part3 = "You suck.";
                            break;
                        case 5:
                            current = combat;
                            Part1 = "Woohaha!";
                            Part2 = "That sounded like it hurt!";
                            break;
                        case 6:
                            current = combat;
                            Part1 = "Imagine that was my hand.";
                            break;
                        case 7:
                            current = combat;
                            Part1 = "Wake me up when you decide to be interesting.";
                            break;
                    }
                }else if (quoteCD == 0 && idleQuoteCD < 1500 && highHype)   //summoner hype
                {
                    Part1 = "";
                    Part2 = "";
                    Part3 = "";
                    int quote = rnd.Next(8);

                    switch (quote)
                    {
                        case 0:
                            current = hypecmbt;
                            Part1 = "Oh...";
                            Part2 = "Oooh... That's...";
                            Part3 = "That's gross.";
                            break;
                        case 1:
                            current = hypecmbt;
                            Part1 = "You're lucky that ain't my hand!";
                            break;
                        case 2:
                            current = hypecmbt;
                            Part1 = "Woah!";
                            Part2 = "You don't need to be THAT viole-";
                            Part3 = "Nevermind.";
                            break;
                        case 3:
                            current = hypecmbt;
                            Part1 = "Look at this lil' guy!";
                            Part2 = "Isn't he cute?";
                            Part3 = "Look at him slap you!";
                            break;
                        case 4:
                            current = hypecmbt;
                            Part1 = "You know";
                            Part2 = "Maybe I'll join too!";
                            break;
                        case 5:
                            current = hypecmbt;
                            Part1 = "Look at them go!";
                            Part2 = "You're SO much bigger than them";
                            Part3 = "And you STILL can't dance!";
                            break;
                        case 6:
                            current = hypecmbt;
                            Part1 = "The dance is deadly!";
                            break;
                        case 7:
                            current = hypecmbt;
                            Part1 = "That's what i call a fight!";
                            break;
                    }
                }

            }
            base.OnHitNPCWithProj(proj, target, damage, knockback, crit);
        }


        //Get closes npc (check the example mod for stuff like this)
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)
                if (target.CanBeChasedBy())
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Player.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }
    }
}
