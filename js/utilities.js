window.syncScroll = {
    init: function (leftId, rightId) {
        const left = document.getElementById(leftId);
        const right = document.getElementById(rightId);

        const sync = (source, target) => {
            // 1. Check if we are at the very top
            if (source.scrollTop <= 5) {
                target.scrollTo({ top: 0, behavior: 'auto' });
                return;
            }

            // 2. Find the FID at the top of the current view
            const topEl = document.elementFromPoint(
                source.getBoundingClientRect().left + 20, 
                source.getBoundingClientRect().top + 60
            );

            const fid = topEl?.closest('[data-fid]')?.getAttribute('data-fid');
            
            if (fid === "TOP") {
                target.scrollTo({ top: 0, behavior: 'auto' });
            } else if (fid) {
                const match = target.querySelector(`[data-fid="${fid}"]`);
                if (match) {
                    // Align the target element to the top of the pane
                    target.scrollTop = match.offsetTop - target.offsetTop;
                }
            }
        };

        left.onscroll = () => sync(left, right);
        right.onscroll = () => sync(right, left);
    }
};
