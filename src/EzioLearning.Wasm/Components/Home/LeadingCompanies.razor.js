export function loadSlider() {
    // Leading Companies

    if ($('.owl-carousel.lead-group-slider').length > 0) {
        var owl = $('.owl-carousel.lead-group-slider');
        owl.owlCarousel({
            margin: 24,
            nav: false,
            dots: false,
            loop: true,
            autoplay: false,
            autoplaySpeed: 2000,
            responsive: {
                0: {
                    items: 3,
                    nav: false,
                    dots: false,
                },
                768: {
                    items: 3,
                    nav: false,
                    dots: false,
                },
                1170: {
                    items: 6,
                    dots: false,
                }
            }
        });
    }

    // Leading Companies

    if ($('.owl-carousel.leading-univercities').length > 0) {
        var owl = $('.owl-carousel.leading-univercities');
        owl.owlCarousel({
            margin: 24,
            nav: false,
            dot: false,
            loop: true,
            autoplay: false,
            autoplaySpeed: 2000,
            responsive: {
                0: {
                    items: 3
                },
                768: {
                    items: 3
                },
                1170: {
                    items: 5
                }
            }
        });
    }

    // Leading Slider

    if ($('.owl-carousel.leading-slider-five').length > 0) {
        var owl = $('.owl-carousel.leading-slider-five');
        owl.owlCarousel({
            margin: 24,
            nav: false,
            loop: true,
            dots: false,
            autoplay: false,
            responsive: {
                0: {
                    items: 2
                },
                768: {
                    items: 3
                },
                1170: {
                    items: 3
                },
                1300: {
                    items: 4
                }
            }
        });
    }
}