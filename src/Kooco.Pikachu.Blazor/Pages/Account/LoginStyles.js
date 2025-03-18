document.addEventListener("DOMContentLoaded", function () {
    const container = document.createElement("div");
    container.classList.add("background-elements");

    container.innerHTML = `
        <div class="background-gradient"></div>
        <div class="blob blob-1"></div>
        <div class="blob blob-2"></div>
        <div class="blob blob-3"></div>
        <div class="blob blob-4"></div>
        <div class="blob blob-5"></div>
        <div class="float-element float-1"></div>
        <div class="float-element float-2"></div>
        <div class="float-element float-3"></div>
        <div class="pulse-light pulse-1"></div>
        <div class="pulse-light pulse-2"></div>
        ${Array(7).fill('<div class="bubble"></div>').join('')}
    `;

    document.body.appendChild(container);
});
