

document.onreadystatechange = () => {
    if (document.readyState === "complete") {
        // document ready
        // hide loading screen
        $(".loader").fadeOut(250);
    }
};
