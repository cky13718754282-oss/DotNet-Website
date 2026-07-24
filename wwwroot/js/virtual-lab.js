(function () {
    "use strict";

    var module = document.querySelector("[data-virtual-lab]");
    if (!module) return;

    var steps = Array.from(module.querySelectorAll("[data-lab-step]"));
    var percentageOutput = module.querySelector("[data-lab-percentage]");
    var countOutput = module.querySelector("[data-lab-count]");
    var progress = module.querySelector("[data-lab-progress]");
    var progressBar = module.querySelector("[data-lab-progress-bar]");
    var completeMessage = module.querySelector("[data-lab-complete]");
    var storageKey = "geekspace-lab-progress-" + module.dataset.resourceId;

    function readProgress() {
        try {
            return JSON.parse(localStorage.getItem(storageKey) || "[]");
        } catch (_) {
            return [];
        }
    }

    function saveProgress() {
        try {
            localStorage.setItem(storageKey, JSON.stringify(steps.map(function (step) {
                return step.checked;
            })));
        } catch (_) {
            // Progress still works for the current page when storage is unavailable.
        }
    }

    function updateProgress(shouldSave) {
        var completed = steps.filter(function (step) { return step.checked; }).length;
        var percentage = Math.round((completed / steps.length) * 100);

        percentageOutput.textContent = percentage + "%";
        countOutput.textContent = completed + " of " + steps.length + " steps";
        progress.setAttribute("aria-valuenow", percentage.toString());
        progressBar.style.width = percentage + "%";
        completeMessage.hidden = completed !== steps.length;

        steps.forEach(function (step) {
            step.closest("li").classList.toggle("step-complete", step.checked);
        });

        if (shouldSave) saveProgress();
    }

    var stored = readProgress();
    steps.forEach(function (step, index) {
        step.checked = stored[index] === true;
        step.addEventListener("change", function () {
            updateProgress(true);
        });
    });

    module.querySelector("[data-lab-reset]").addEventListener("click", function () {
        steps.forEach(function (step) { step.checked = false; });
        updateProgress(true);
        module.scrollIntoView({ behavior: "smooth", block: "start" });
    });

    updateProgress(false);
})();
