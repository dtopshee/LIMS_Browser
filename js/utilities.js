window.syncScroll = {
    init: function (leftId, rightId) {
        const leftPane = document.getElementById(leftId);
        const rightPane = document.getElementById(rightId);

        const sync = (source, target) => {
            // Find the element currently at the top of the source pane
            const topElement = document.elementFromPoint(
                source.getBoundingClientRect().left + 10, 
                source.getBoundingClientRect().top + 50
            );
            
            const fid = topElement?.closest('[data-fid]')?.getAttribute('data-fid');
            if (fid) {
                const targetElement = target.querySelector(`[data-fid="${fid}"]`);
                if (targetElement) {
                    targetElement.scrollIntoView({ behavior: 'auto', block: 'start' });
                }
            }
        };

        leftPane.onscroll = () => sync(leftPane, rightPane);
        // Optional: rightPane.onscroll = () => sync(rightPane, leftPane);
    }
};
