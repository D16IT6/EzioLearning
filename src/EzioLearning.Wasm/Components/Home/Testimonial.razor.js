export function loadSlider() {
    if ($('.testimonial-five.lazy').length > 0) {
        $(".testimonial-five.lazy").slick({
            lazyLoad: 'ondemand',
            slidesToShow: 1,
            slidesToScroll: 1,
            autoplay: false,
            autoplaySpeed: 0,
            speed: 3000,
            autoplaySpeed: 1800

        });
    }
    // Testimonial Slider

    if ($('.testimonial-slider').length > 0) {
        $('.testimonial-slider').owlCarousel({
            loop: true,
            margin: 15,
            dots: true,
            nav: false,
            smartSpeed: 10000,
            dotsSpeed: 1000,
            dragEndSpeed: 1000,
            responsive: {
                0: {
                    items: 1,

                },
                500: {
                    items: 1,

                },
                768: {
                    items: 1,

                },
                1000: {
                    items: 1,

                },
                1300: {
                    items: 1,

                }
            }
        })
    }
}