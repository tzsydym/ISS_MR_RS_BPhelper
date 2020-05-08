$("#startCrawler").click(function () {

    region = $(".dropdown-trigger[data-target ='dropdownServer_crawl']").children('div').text();
    rank = $(".dropdown-trigger[data-target ='dropdownRank_crawl']").children('div').text();
    if (region != '' && rank != '')
    {
        setTimeout(function () {
            var para = JSON.stringify({ 'message': 'check data existence', 'region': region, 'rank': rank });
            var promise = boundedObj.requestService(para);
            promise.then(function (value) {
                if (value == "Csv file already exists.") {
                    var toastHTML = '<span>' + value + ' Do you want to download and replace it?' +'</span><button class="btn-flat toast-action" onclick="confirmReplacingCsv()">Yes</button><button class="btn-flat toast-action" onclick="M.Toast.dismissAll();">No</button>';
                    M.toast({ html: toastHTML, timeRemaining:15000 });
                }
                else
                {
                    M.toast({ html: value, classes: 'rounded' });
                    $('.overlay').toggleClass('scale-out');
                }                 
            });
        }, 500); 
    }        
});
function confirmReplacingCsv()
{
    $('.overlay').toggleClass('scale-out');
    region = $(".dropdown-trigger[data-target ='dropdownServer_crawl']").children('div').text();
    rank = $(".dropdown-trigger[data-target ='dropdownRank_crawl']").children('div').text();
    var para = JSON.stringify({ 'message': 'start crawler', 'region': region, 'rank': rank });
    var promise = boundedObj.requestService(para);
    promise.then(function (value) {
        M.toast({ html: value, classes: 'rounded' });
        setTimeout(function () {
            $('.overlay').toggleClass('scale-out');
        }, 100);
    });
}
$("#persistCsvData").click(function () {
    M.toast({ html: "Persisting data", classes: 'rounded' });
    $('.overlay').toggleClass('scale-out');
    region = $(".dropdown-trigger[data-target ='dropdownServer_persist']").children('div').text();
    rank = $(".dropdown-trigger[data-target ='dropdownRank_persist']").children('div').text();
    var para = JSON.stringify({ 'message': 'persist data', 'region': region, 'rank': rank });
    var promise = boundedObj.requestService(para);
    promise.then(function (value) {
        M.toast({ html: value, classes: 'rounded' });
        setTimeout(function () {
            $('.overlay').toggleClass('scale-out');
        }, 100);
    });
});
$('.dropdown-trigger').dropdown({ container: $('#pageContent'), constrainWidth:false});
$('.modal').modal({
    opacity: 0,
});

$('.dropdown-content li div a').click(function (event) {
    imgsrc = $(this).siblings('img').attr('src');
    imgname = $(this).text();
    chiphtml = getChipHtml(imgsrc, imgname);
    dropdownId = $(this).closest('.dropdown-content').attr("id");;
    $(".dropdown-trigger[data-target ='" + dropdownId + "']").html(chiphtml);
})

function getChipHtml(imgsrc,imgname)
{
    chip = "<div class='chip' style='color:white;margin-right:0px;background-color:transparent'>" +
        "<img src='" + imgsrc + "'style='padding-left:0px;margin-top:2px;height:28px;width:28px'>" +
        imgname+
        "</div >" ;
    return chip;
}