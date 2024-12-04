window.PrintTradeDocument = function (html) {
    const newTab = window.open('', '_blank');

    newTab.document.write(html);

    newTab.document.close();
}