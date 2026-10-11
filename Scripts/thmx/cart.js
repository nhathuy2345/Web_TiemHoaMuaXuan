$(document).ready(function () {
    // Thêm vào giỏ hàng bằng AJAX
    $('.btn-add-to-cart').click(function (e) {
        e.preventDefault();
        var productId = $(this).data('id');
        var quantity = $('#quantity-input').val() || 1;

        $.ajax({
            url: '/Cart/AddToCart',
            type: 'POST',
            data: { productId: productId, quantity: quantity },
            success: function (res) {
                if (res.success) {
                    $('#cart-badge-count').text(res.count);
                    alert('Đã thêm sản phẩm vào giỏ hàng!');
                }
            },
            error: function () {
                alert('Có lỗi xảy ra, vui lòng thử lại!');
            }
        });
    });
});