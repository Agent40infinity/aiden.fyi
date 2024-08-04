//Systems
var loadState = { FirstTime: 1, Default: 2, Information: 3 };
let portfolioState = loadState.FirstTime;
let visited = localStorage.getItem("Visited");

if (visited == "true") 
{
    portfolioState = loadState.Default 
}

// Audio
var masterAudio = new Audio('../Resources/Media/landing.mp3');
var dBConversion = 250;
var lastVolume = 0;

// Menu
var menuToggle = false;
var topAnimateDist = 200;
var headerAnimateDist = 400;

window.onbeforeunload = function () {
    window.scrollTo(0,0);
};

$(document).ready(function() 
{
    masterAudio.volume = 0.4;

    switch (portfolioState)
    {
        case loadState.FirstTime:
            FirstTimeLoad();
            break;
        case loadState.Default:
            $('.banner').css({"display": "none"});
            AddContents();
            DefaultAudio();
            break;
    }

    // Animates the header once too far down.
    // Makes the header / navigation banner transparent.
    $(window).scroll(function() {
        var screenTop = $(window).scrollTop();

        if (screenTop >= topAnimateDist) { 
            $('.return').fadeIn(200);
        } 
        else {    
            $('.return').fadeOut(200);
        }

        if (screenTop >= headerAnimateDist)
        {
            $('header').css({"background-color": "rgba(0, 0, 0, 0.7)", "transition": "0.2s"});
        }
        else
        {
            $('header').css({"background-color": "rgba(0, 0, 0, 1)", "transition": "0.2s"});
        }
    });

    // I can't remember what this does for the life of me.
    $(".arrow").on("click", function() {
        element = 0;
        scrollElement = "";

        if ($(this).hasClass('special')) {
            element = "1";  
        }
        else
        {
            scrollElement = $(this).parent().attr('id');
            element = parseInt(scrollElement.replace(/[^0-9]/g, '')) + 1; 
            console.log(element);
        }

        $([document.documentElement, document.body]).animate({scrollTop: $("#chapter" + element).offset().top - 125}, 1000);
        console.log("#" + element);
    });

    // Clicking on the comments icon will scroll you down to the 'Contact Me' footer.
    $(".comments").on("click", function() {
        window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' });
    });

    // Both of these are used to fade in and out the volume slider.
    // muteToggle fades in the slider.
    $(".muteToggle").mouseover(function () { 
        $(".slide-hide").fadeIn(200);
    });

    //volumeContainer fades out the slider when the user isn't over both objects.
    $(".volume-container").mouseleave(function () { 
        $(".slide-hide").fadeOut(200);
    });

    //Binds the slider value to a progression bar to link the audio volume with the audio playing.
    volume = document.getElementById("slider");
    
    volume.oninput = function() {
        value = document.getElementById("slider-value");
        value.innerHTML = slider.value;
        masterAudio.volume = slider.value / dBConversion;
        updateProgress();
    }
});

function FirstTimeLoad()
{
    $(".enter").one("click", function () {
        playAudio();

        $('.hidden').not('hr').animate({opacity: 1}, 3000);
        $('hr.hidden').animate({width: '40%', opacity: 1}, 2000);
        $('.enter').animate({opacity: 0}, 1000);

        setTimeout(function() {
            $('.banner').fadeOut(1000);

            setTimeout(function() {
                AddContents();
                localStorage.setItem("Visited", true);
            }, 1000);
        }, 3500);
    });
}

function AddContents()
{
    $('header').animate({width: '100%', opacity: 1}, 2000);
    $('.volume-container').fadeIn(2000);
    $('.volume-container').css({display: "flex"});
    $('.background').animate({top: "505px"}, 1000);
    $('.enter').fadeOut(0);
    $('.define').animate({opacity: 1}, 2000);

    setTimeout(function() {
        $('.special').fadeIn(1000);
        $('.information').fadeIn(2000);
        $('footer').fadeIn(2000);
        $('body').css("overflow-y" , "visible");
    }, 200);
}

// Used to toggle the audio.
function toggleMute(img) {
    if (masterAudio.muted)  {
        if (visited == "true" && masterAudio.paused) {
            playAudio();
        }

        masterAudio.muted = false;
        lastAudio(true);
        img.src="../Resources/SVGs/Speaker_Icon.svg";
    }
    else {
        masterAudio.muted = true;
        lastAudio(false);
        img.src="../Resources/SVGs/Mute_Icon.svg";
    }

    updateProgress();
}

function playAudio() {
    masterAudio.play();
    masterAudio.loop = true;
}

function DefaultAudio() {
    var mute = $('.muteToggle').first();
    toggleMute(mute);
    mute.attr("src","../Resources/SVGs/Mute_Icon.svg");
}

// Marks the last known audio value so that when muting and unmuting, it returns to the original value.
function lastAudio(toggle) {
    switch (toggle)
    {
        case true:
            masterAudio.volume = lastVolume;
            break;
        case false:
            lastVolume = masterAudio.volume;
            masterAudio.volume = 0;
            break;
    }

    value = document.getElementById("slider-value");
    value.innerHTML = masterAudio.volume * dBConversion;
    slider = document.getElementById("slider");
    slider.value = masterAudio.volume * dBConversion;
}

// Updates the volume of the audio using the slider after calculating the correct conversion.
function updateProgress() {
    for (let e of document.querySelectorAll('input[type="range"].slider-progress')) {
        e.style.setProperty('--value', e.value);
        e.style.setProperty('--min', e.min == '' ? '0' : e.min);
        e.style.setProperty('--max', e.max == '' ? '100' : e.max);
        e.addEventListener('input', () => e.style.setProperty('--value', e.value));
    }
}

// Used to return to the top of the page.
function returnTop() {
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// Utilises the arrows to auto scroll to x chapter.
function scrollChapter(chapter) {
    $([document.documentElement, document.body]).animate({scrollTop: $("#chapter" + chapter).offset().top - 125}, 1000);
}

// Toggles the menu.
function toggleMenu() {
    switch (menuToggle)
    {
        case false:
            document.getElementById("ham-nav").style.width="600px";
            menuToggle = true;
            break;
        case true:
            document.getElementById("ham-nav").style.width="0px";
            menuToggle = false;
            break;
    }
}