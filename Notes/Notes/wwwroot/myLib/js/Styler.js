
function myFunction() {
        var e = document.getElementById("myStyler");
        var backgroundName = e.options[e.selectedIndex].value;

        document.getElementById("myBody").style.background = backgroundName;              
}