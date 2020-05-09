

## SECTION 1 : PROJECT TITLE
## LOL Smart Ban/Pick Assistant

<img src="SystemCode/Screenshot 2020-05-07 at 4.00.24 PM.png"
     style="float: left; margin-right: 0px;" />

---

## SECTION 2 : EXECUTIVE SUMMARY / PAPER ABSTRACT
From 2010 until today, League of Legends (LOL) is certainly one of the most popular multiplayer online battle arena video games out there, with more than 100 million active players across the world, it had won the ‘Best eSports Game’ and ‘Best eSports Event’ in The Game Awards 2019 competition and countless other awards from past few years. It is for sure that most of us have heard of this game and many our friends or even ourselves actually play the game. There are many reasons behind the success of League of Legends, it is a totally free-to-play game where players do not have to spend a single cent and he is able to enjoy the game just like others. It also has fantastic visual effects and does not require high hardware specs from the PC. 


But the most important reason for the game to continually being the top video game in the world is the nature of the game: teamwork, competitiveness and fast pace. It is a 5 vs 5 multiplayer game that each player will select a champion and work together with his team intensively to destroy enemy team’s towers. Each player’s champion will have its own unique skills and stats and the player has to find the best hero that matches his team’s draft and at the same time it is the best against the enemy’s draft during the ban/pick phase.


This is where our product comes in to help, with nearly 150 champions available in the pool, players normally pick and ban champions based on their own experience and understanding. But with such big amount of choices, player may not have a clear sight on which champions to pick so it best matches their own draft and in the meanwhile best counter the enemy’s team, and which champions to ban from the pool so the enemy team is not able to pick them. It is a very complex network of counter and coordinate relations, and our product aims to help players with this process of decision making by using the knowledge extracted from the game data we collected from the community. To assist the players come up with the best draft and ban list that leads to the highest overall win rate.



---

## SECTION 3 : CREDITS / PROJECT CONTRIBUTION

| Official Full Name  | Student ID (MTech Applicable)  | Work Items (Who Did What) | Email (Optional) |
| :------------ |:---------------:| :-----| :-----|
| Wen Cheng | A0213572L  | Idea Design, Data Crawler Implementation, Project Presentation & Video| e0508673@u.nus.edu |
| Du Yiming | A0165454B | System design, Rule system & Ban/Pick UI Implementation | e0167328@u.nus.edu |


---

## SECTION 4 : VIDEO OF SYSTEM MODELLING & USE CASE DEMO

[![LOL Smart Ban/Pick Assistant](https://img.youtube.com/vi/-BvLvy7L5os/maxresdefault.jpg)](https://youtu.be/-BvLvy7L5os "LOL Smart Ban/Pick Assistant")
(Click above image for the link to the video)

---

## SECTION 5 : USER GUIDE

`Refer to <Installation and User Guide> in project report at Github Folder: ProjectReport`

### For detailed step with picture reference, please refer to Installation and User Guide in project report at Github Folder: ProjectReport
Installation and User Guide to LOL Smart Ban/Pick Assistant
1.	In a Windows PC, download the ‘debug.zip’ file from https://github.com/tzsydym/ISS_MR_RS_BPhelper/blob/master/debug.zip 
2.	Unzip the file to any location.
3.	To launch the application, double click on the ‘ISS_MS_RS_BpHelper’ file  
4.	Before using the application, please use the data crawler module to extract the data set, go the crawler page by opening the side menu
 
5.	Click Menu > Start Crawler
 
6.	Select region and rank based on own preference
 
7.	Click ‘OK’ once selection done
 
8.	Wait for the crawler to finish the job (could take a few minutes)
9.	After the job finished, make sure 2 csv files are under ‘debug\crawler data\(Server selected)\(rank selected)’ 
 
10.	Go back to the crawler page and click ‘menu again’, then click ‘Persist csv into memory’ to load the data into the application
 
11.	Then select the server and rank from step 6/7 (or other server/rank that are available in  ‘debug\crawler data’ directory)
 
12.	Now we are ready to use the ban/pick assistant, go to side menu and click ‘Home’
 
13.	Click ‘start new game’ button at the bottom right corner to start a ban/pick session
 
14.	The left side is our team’s ban/pick and the right side is enemy team, the flick symbol means who’s turn to ban/pick. 
 
15.	Now go ahead and play with our tool and see what happened, the chart will be updated dynamically for each selection to show the relation values, you can make your ban/pick selection by clicking the button under the chart
 


---
## SECTION 6 : PROJECT REPORT / PAPER

`Refer to project report at Github Folder: ProjectReport`

Please find project report at https://github.com/tzsydym/ISS_MR_RS_BPhelper/blob/master/ProjectReport/IRS-MRS-2020-01-18-IS02PT-GRP-20-LOL_Smart_Ban:Pick_Assistant.pdf

---
## SECTION 7 : MISCELLANEOUS

`Refer to Github Folder: Miscellaneous`

### overallChampionsInfo.csv
* Overall information for each champions in North America server, Gold rank
* Win rate, ban rate, lane (role)

### championCounter.csv
* champion counter/teammate relations
* what is each champion 'countered by', 'best against' and 'best with'


