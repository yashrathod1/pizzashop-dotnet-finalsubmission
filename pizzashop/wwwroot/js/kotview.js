
let currentstatus = "In Progress";

function loadOrders(categoryId, targetSelector, status) {
    $.ajax({
        url: '/KOT/GetOrderCardByCategory',
        type: 'GET',
        data: { categoryId: categoryId, status: status },
        success: function (res) {
            $(targetSelector).html(res);
            initializeSlider($(targetSelector).find('.sliderContainer'));
            initLiveTimers();
        }
    });
}


function initializeSlider($sliderContainer) {

    let currentSlide = 0;
    const cardWidth = 287;
    const cardsToShow = 1;
    const maxSlide = $sliderContainer.children().length - cardsToShow;

    const $tab = $sliderContainer.closest('.tab-pane');

    const $nextBtn = $tab.find('.nextBtn');
    const $prevBtn = $tab.find('.prevBtn');

    $nextBtn.off('click').on('click', function () {
        if (currentSlide < maxSlide) currentSlide++;
        else currentSlide = 0;
        const offset = -currentSlide * cardWidth;
        $sliderContainer.css('transform', `translateX(${offset}px)`);
    });

    $prevBtn.off('click').on('click', function () {
        if (currentSlide > 0) currentSlide--;
        else currentSlide = maxSlide;
        const offset = -currentSlide * cardWidth;
        $sliderContainer.css('transform', `translateX(${offset}px)`);
    });
}


$(document).ready(function () {

    

    $('.tab-pane').each(function () {
        const $tab = $(this);
        if ($tab.find('.status-btn.active').length === 0) {
            $tab.find('.status-btn').first().addClass('active');
        }
    });

    const initialStatus = $('#sliderContainer').closest('.tab-pane').find('.status-btn.active').data('status');
    loadOrders(null, '#sliderContainer', initialStatus);


    $(document).on('click', '.nav-link', function () {
        const targetId = $(this).data('bs-target');
        const $target = $(targetId);
        const $container = $target.find('.order-card-container');
        const categoryId = $container.data('category-id');

        const status = $target.find('.status-btn.active').data('status');

        const containerSelector = $container.length > 0
            ? `${targetId} .order-card-container`
            : '#sliderContainer';
        loadOrders(categoryId, containerSelector, status);
    });
});

$(document).on('click', '.status-btn', function () {
    const $this = $(this);
    const $tab = $(this).closest('.tab-pane');
    $tab.find('.status-btn').removeClass('active');
    $this.addClass('active');

    const status = $this.data('status');
    if ($tab.attr('id') === 'all') {
        const containerSelector = '#sliderContainer';
        loadOrders(null, containerSelector, status);
    } else {
        const categoryId = $tab.find('.order-card-container').data('category-id');
        const containerSelector = `#${$tab.attr('id')} .order-card-container`;
        loadOrders(categoryId, containerSelector, status);
    }
});


$(document).on('click', '.quantity-increase', function () {
    let box = $(this).closest('.quantity-box');
    let valueElem = box.find('.quantity-value');
    let current = parseInt(valueElem.text());
    let max = parseInt(box.data('max'));
    if (current < max) {
        valueElem.text(current + 1);
    }
});

$(document).on('click', '.quantity-decrease', function () {
    let box = $(this).closest('.quantity-box');
    let valueElem = box.find('.quantity-value');
    let current = parseInt(valueElem.text());
    let min = parseInt(box.data('min'));
    if (current > min) {
        valueElem.text(current - 1);
    }
});


$(document).on('click', '.order-card', function () {
    const orderId = $(this).data('order-id');
    const $card = $(this);
    const $tab = $card.closest('.tab-pane');
    const status = $tab.find('.status-btn.active').data("status");

    $('#currentOrderStatus').val(status);

    if (status == "In Progress") {
        $('#markReadyBtnText').text('Mark as Prepared');
    } else if (status == "Ready") {
        $('#markReadyBtnText').text('Mark as In Progress');
    }
    $('#orderModalLabel').text('Order ID: #' + orderId);

    $.ajax({
        url: '/KOT/GetOrderCardInModal',
        type: 'GET',
        data: { orderId, status },
        success: function (result) {
            $('#orderModalBody').html(result);
            $('#orderModal').modal('show');
        },
        error: function () {
            $('#orderModalBody').html('<div class="text-danger">Error loading order details.</div>');
            $('#orderModal').modal('show');
        }
    });
});

$(document).on('click', '#markReadyBtn', function () {
    const orderIdText = $('#orderModalLabel').text();
    const orderId = parseInt(orderIdText.replace('Order ID: #', ''));
    const status = $('#currentOrderStatus').val();

    const items = [];

    $('#orderModalBody .order-item-row').each(function () {

        const isChecked = $(this).find('.form-check-input').is(':checked');
        if (isChecked) {
            const itemId = $(this).data('item-id');
            const changeInQuantity = parseInt($(this).find('.quantity-value').text());

            items.push({
                itemId: itemId,
                changeInQuantity: changeInQuantity
            });
        }
    });

    $.ajax({
        url: '/KOT/UpdatePreparedQuantities',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ orderId: orderId, items: items, status: status }),
        success: function () {
            $('#orderModal').modal('hide');

            const currentTab = $('.tab-pane.active');
            const tabSelector = `#${currentTab.attr('id')}`;
            const container = currentTab.find('.order-card-container');

            if (container.length > 0) {
                const categoryId = container.data('category-id');
                loadOrders(categoryId, `${tabSelector} .order-card-container`, status);
            } else {
                loadOrders(null, `${tabSelector} #sliderContainer`, status);
            }
        },
        error: function () {
            alert("Error updating order!");
        }
    });
});

//live timer
function initLiveTimers() {
    $('.live-timer-kot').each(function () {
        var $this = $(this);
        var startTimeStrr = $this.data('starttime');

        // Ensure startTime is a valid date
        var startTime = new Date(startTimeStrr);

        var updateTimer = function () {
            var now = new Date();
            var diff = now - startTime;
            var totalSeconds = Math.floor(diff / 1000);

            var days = Math.floor(totalSeconds / (3600 * 24));
            var hours = Math.floor((totalSeconds % (3600 * 24)) / 3600);
            var minutes = Math.floor((totalSeconds % 3600) / 60);
            var seconds = totalSeconds % 60; 


            $this.text(days + " days " + hours + " hours " + minutes + " min " + seconds + " sec");
        };
        updateTimer();
        setInterval(updateTimer, 1000);
    });
}
