function getAvatars()
{
    var championPool = $("#champion_pool");
    championPool.fadeToggle('fast', function () {
        championPool.empty();
        var para = JSON.stringify({ 'message': 'get avatars' });
        var promise = boundedObj.requestService(para);
        promise.then(function (value) {
            var avatars = JSON.parse(value);
            var d1 = $.Deferred();
            championPool.fadeToggle('fast', function () {
                for (var i = 0; i < avatars.length -1; i++) {
                    var champ = generateAvatar(avatars[i]);
                    $(champ).hide().appendTo(championPool).fadeIn(1000);
                    if (i == avatars.length - 2)
                    {
                        d1.resolve();
                    } 
                }
                $.when(d1).done(function () {
                    $("#champion_pool .champion-card").click(function () {
                        var name = $(this).attr('data-tooltip');
                        if (instruction != null) {
                            if (!$(this).hasClass('banned'))
                            {
                                attachChampionInfo(name);
                            }
                        }
                    });
                    initToolTip();
                });
                
            });

        });
    });
    
}
$('#pickOrBan').click(function () {
    if (instruction != null)
    {
        if ($('.pulse').length > 0) {
            var name = $('.pulse').siblings('img').attr('alt');
            $('#champion_pool').find('div[data-tooltip=' + '"' + name + '"').addClass('banned');
            if (BP[instruction.bPPhase] == "ban") {
                var ourBanSlots = $('#ourTeamBaned div');
                var enemyBanSlots = $('#enemyTeamBaned div');
                if (instruction.rotate == 0) {
                    $(ourBanSlots[instruction.teammateID]).find('img').attr('src', '../IMG/champion-avatar/' + name + '.jpg');
                    var imgsrc = '../IMG/leftTeamDefault.png';
                    changePlayerAvatar(imgsrc, "");
                    $('.pulse').siblings('span').text("Waitting to pick");
                }
                else {
                    $(enemyBanSlots[instruction.teammateID]).find('img').attr('src', '../IMG/champion-avatar/' + name + '.jpg');
                    var imgsrc = '../IMG/rightTeamDefault.png';
                    changePlayerAvatar(imgsrc, "");
                    $('.pulse').siblings('span').text("Waitting to pick");
                }
            }
            else {
                $('.pulse').siblings('span').text(name);
            }
            var para = JSON.stringify({ 'message': 'take action', selectedChampion: name });
            var promise = boundedObj.requestService(para);
            promise.then(function (value) {
                instruction = JSON.parse(value);
                updateRecommendList();
                updateUI();
            });
        }
        else
        {
            M.toast({ html: "Ban and pick has finished.", classes: 'rounded' });
        }
    }
})
$('#startNewGame').click(function () {
    removeData(chart_champion_1);
    removeData(chart_champion_2);
    var ourBanSlots = $('#ourTeamBaned div');
    var enemyBanSlots = $('#enemyTeamBaned div');
    ourBanSlots.each(function (index) {
        $(this).find('img').attr('src', '../IMG/banedDefaultLeft.png');
    })
    enemyBanSlots.each(function (index) {
        $(this).find('img').attr('src', '../IMG/banedDefaultRight.png');
    })

    $('.grow.champion-card').removeClass('banned');
    var lanes = ["Top", "Mid", "ADCarry", "Jungle", "Support"];
    var ourLanes_i = [], ourLanes = new Array(5);
    var enemyLanes_i = [], enemyLanes = new Array(5);
    while (ourLanes_i.length < 5) {
        var r = Math.floor(Math.random() * 5);
        if (ourLanes_i.indexOf(r) === -1) ourLanes_i.push(r);
        
    }
    while (enemyLanes_i.length < 5) {
        var r2 = Math.floor(Math.random() * 5);
        if (enemyLanes_i.indexOf(r2) === -1) enemyLanes_i.push(r2);

    }
    console.log(ourLanes_i)
    console.log(enemyLanes_i)
    for (var i = 0; i < 5; i++) {
        ourLanes[i] = lanes[ourLanes_i[i]];
        enemyLanes[i] = lanes[enemyLanes_i[i]];
    }
    //var lanes = ["Top", "Mid", "ADCarry", "Jungle", "Support"];
    //var ourLanes = new Array(5);
    //var enemyLanes = new Array(5);
    //for (var i = 0; i < 5; i++)
    //{
    //    ourLanes[i] = lanes[Math.floor(Math.random() * 5)];   
    //    enemyLanes[i] = lanes[Math.floor(Math.random() * 5)];   
    //}
    var ourTeam = $('#ourTeam li');
    var enemyTeam = $('#enemyTeam li');
    ourTeam.each(function (index) {
        $(this).find('p img').attr('src', '../IMG/' + ourLanes[index].toLowerCase() + '.png')
        $(this).find('p span').text(ourLanes[index]);
    })
    enemyTeam.each(function (index) {
        $(this).find('p img').attr('src', '../IMG/' + enemyLanes[index].toLowerCase() + '.png')
        $(this).find('p span').text(enemyLanes[index]);
    })
    var firstMove = Math.random() >= 0.5;
    var para = JSON.stringify({ 'message': 'start new game', 'ourLane': ourLanes, 'enemyLane': enemyLanes, 'firstMove': firstMove});
    var promise = boundedObj.requestService(para);
    promise.then(function (value) {
        if (value == "Data has not been persisted yet.") {
            M.toast({ html: value, classes: 'rounded' });
        }
        else
        {
            instruction = JSON.parse(value);
            updateRecommendList();
            updateUI();
        }
    });
})
function changePlayerAvatar(imgsrc,name)
{
    var ourTeam = $("#ourTeam li");
    var enemyTeam = $("#enemyTeam li");

    if (instruction.rotate == 0) {
        $(ourTeam[instruction.teammateID]).find('.circle').attr('src', imgsrc);
        $(ourTeam[instruction.teammateID]).find('.circle').attr('alt', name);
    }
    else {
        $(enemyTeam[instruction.teammateID]).find('.circle').attr('src', imgsrc );
        $(enemyTeam[instruction.teammateID]).find('.circle').attr('alt', name);
    }
}
function attachChampionInfo(name)
{
    removeData(chart_champion_1);
    removeData(chart_champion_2);
    var champion;
    for (var i = 0; i < instruction.champions.length; i++)
    {
        if (instruction.champions[i].Name == name)
        {
            champion = instruction.champions[i];
        }
    }
    var imgsrc = '../IMG/champion-avatar/' + champion.Name + '.jpg';
    var name = champion.Name;
    changePlayerAvatar(imgsrc, name);
    addData(chart_champion_1, generateDataset(champion,1));
    addData(chart_champion_2, generateDataset(champion,2));
}

function updateUI()
{
    console.log(instruction);
    $("#pickOrBan").text(BP[instruction.bPPhase]);
    $(".secondary-content.btn-floating").removeClass('pulse').addClass('btn-small');
    var ourTeam = $("#ourTeam li");
    var enemyTeam = $("#enemyTeam li");
    
    if (instruction.rotate == 0) {
        $(ourTeam[instruction.teammateID]).find('.title').text(BP_ing[instruction.bPPhase]);
        $(ourTeam[instruction.teammateID]).find('a').addClass('pulse').removeClass('btn-small');
    }
    else
    {
        $(enemyTeam[instruction.teammateID]).find('.title').text(BP_ing[instruction.bPPhase]);
        $(enemyTeam[instruction.teammateID]).find('a').addClass('pulse').removeClass('btn-small');
    }
}
function updateRecommendList()
{
    $("#recommendList div").fadeOut(500, function () {
        $("#recommendList div").remove();
        for (var i = 0; i < instruction.recommendedChampions.length; i++) {
            var chip = '<div class="chip grow">' +
                '<img src = "../IMG/Champion-avatar/' + instruction.recommendedChampions[i].Name +
                '.jpg" alt="' + instruction.recommendedChampions[i].Name + '">'+
                instruction.recommendedChampions[i].Weight +
                '</div>';
            $(chip).hide().appendTo($("#recommendList")).fadeIn(300);
        }
        $("#recommendList div").click(function () {
            var name = $(this).find('img').attr('alt');
            if (instruction != null) {
                attachChampionInfo(name);
            }
        })
    });
}
function generateAvatar(name)
{
    name.replace("'", "\'");
    var name_nofileExtension = "";
    var name_splited = name.split(".");

    for (var i = 0; i < name_splited.length -1; i++)
    {
        //Dr.Mundo is a strange name lol
        var dot = ".";
        if (i == name_splited.length - 2)
        {
            dot = "";
        }
        name_nofileExtension += name_splited[i] + dot;
    }
    var card =
        '<div class="card-panel tooltipped col s2 m2 l2 grow champion-card" style ="padding: 10px;width:15%;" data-position="top" data-tooltip="' + name_nofileExtension+'">' +
        '<img src ="../IMG/champion-avatar/' + name + '" alt="' + name_nofileExtension+'">' +
        '</div>'
    return card;
}