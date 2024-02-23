export function loadSlider() {
    if ($('.owl-carousel.home-three-trending-course').length > 0) {
        var owl = $('.owl-carousel.home-three-trending-course');
        owl.owlCarousel({
            margin: 25,
            nav: true,
            //nav: true,
            loop: true,
            responsive: {
                0: {
                    items: 1
                },
                500: {
                    items: 1,

                },
                768: {
                    items: 1,

                },
                1000: {
                    items: 3,

                },
                1300: {
                    items: 4,

                }
            }
        });
    }

    if ($('.owl-carousel.trending-course').length > 0) {
        var owl = $('.owl-carousel.trending-course');
        owl.owlCarousel({
            margin: 24,
            nav: false,
            //nav: true,
            loop: true,
            responsive: {
                0: {
                    items: 1
                },
                768: {
                    items: 2
                },
                1170: {
                    items: 3
                }
            }
        });
    }
    // Feature-instructor-two-slider

    if ($('.feature-instructor-two-slider').length > 0) {
        $('.feature-instructor-two-slider').owlCarousel({
            loop: true,
            margin: 15,
            dots: true,
            nav: false,
            responsive: {
                0: {
                    items: 1,

                },
                500: {
                    items: 1,

                },
                768: {
                    items: 2,

                },
                1000: {
                    items: 3,

                },
                1300: {
                    items: 4,

                }
            }
        });

        // Feature Instructors

        if ($('.owl-carousel.instructors-course').length > 0) {
            var owl = $('.owl-carousel.instructors-course');
            owl.owlCarousel({
                margin: 24,
                //nav: false,
                nav: true,
                loop: true,
                responsive: {
                    0: {
                        items: 1
                    },
                    768: {
                        items: 2
                    },
                    1170: {
                        items: 4
                    }
                }
            });
        }
    }

    if ($('.owl-carousel.instructors-course').length > 0) {
        var owl = $('.owl-carousel.instructors-course');
        owl.owlCarousel({
            margin: 24,
            nav: false,
            nav: true,
            loop: true,
            responsive: {
                0: {
                    items: 1
                },
                768: {
                    items: 2
                },
                1170: {
                    items: 4
                }
            }
        });
    }

}