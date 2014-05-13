Yahtzee AI
=========

This project was designed in 2013 as a testament to a computer's abilities to beat humans (e.g. parents, bosses) consistently at a game of chance.  The result: Yahtzee AI, with an average score of 186 points per round.  (Oh, and it best a human 80% of the time.)

Development Status:**Stable**

How It Works
-----------
1. For every dice roll, the computer randomly selects five numbers using a cryptographic random number generator.
2. Threads look for obvious tests such as "Is my roll a Yahtzee?" and skip the next two steps if they pass.
2. 32 simultaneous threads look for the optimal strategy by dropping or keeping certain dice.
	5. Threads give a relative score for each strategy (Ones, Twos, Three of a Kind, etc.) based on the possible dice score.
	6. Threads select the strategy with the highest score and return its score with the selected strategy.
4. The greatest score from all the threads becomes the active strategy and its dice are dropped (ex: thread 10 will keep only dice 2 and 4 - 01010)
5. The process repeats for roll two.
6. After the final (third) roll, the system then looks at all the dice to determine the best slot to fill.

Contributing
----------
The project was marked unmaintained as of August 2013, but you can still contribute!  If you find a bug fix, feel free to report it or go inside and fix it yourself!  Find a better way to do something?  Let us know!

If you want to add code yourself, all you need is a copy of [Visual Studio](http://visualstudio.com) with [.NET Framework 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21) and [PowerPacks](http://www.microsoft.com/en-us/download/details.aspx?id=25169).  When your changes have been made, right-click on the project, select Publish, and create the executable!

Applications
----------
Artificial Intelligence is no simple project.  While the score-based technique, known as "cognitive computing", used in this AI may only easily apply to other luck-based problems, it is present even further in the works of [IBM Watson](https://www.youtube.com/watch?v=DywO4zksfXw) and can be ultimately applied to any number of fields.  There are many other methods of arriving at a single answer and if you want to see more, [take a quick peek as to what the future may bring](http://www.alexwg.org/publications/PhysRevLett_110-168702.pdf).


----------
Arthur Pachachura (smo-key), March 2014
