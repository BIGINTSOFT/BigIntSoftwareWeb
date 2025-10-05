// BigInt Software ERP - Custom JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Sidebar toggle for mobile
    const mobileMenuBtn = document.getElementById('mobileMenuBtn');
    const sidebar = document.getElementById('sidebar');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    
    if (mobileMenuBtn && sidebar) {
        mobileMenuBtn.addEventListener('click', function() {
            sidebar.classList.toggle('show');
            if (sidebarOverlay) {
                sidebarOverlay.classList.toggle('show');
            }
        });
    }
    
    // Close sidebar when clicking overlay
    if (sidebarOverlay) {
        sidebarOverlay.addEventListener('click', function() {
            sidebar.classList.remove('show');
            sidebarOverlay.classList.remove('show');
        });
    }
    
    // Sidebar toggle for desktop (collapse/expand)
    const sidebarToggleBtn = document.getElementById('sidebarToggleBtn');
    const sidebarToggleIcon = document.getElementById('sidebarToggleIcon');
    const mainContent = document.querySelector('.main-content');
    
    if (sidebarToggleBtn && sidebarToggleIcon && mainContent) {
        // Toggle sidebar function
        function toggleSidebar() {
            sidebar.classList.toggle('collapsed');
            mainContent.classList.toggle('expanded');
            
            // Change icon and add blinking effect when collapsed
            if (sidebar.classList.contains('collapsed')) {
                sidebarToggleIcon.className = 'bi bi-layout-sidebar sidebar-icon-collapsed';
                sidebarToggleBtn.title = 'Sidebar\'ı Aç';
                sidebarToggleBtn.classList.add('blinking');
            } else {
                sidebarToggleIcon.className = 'bi bi-layout-sidebar';
                sidebarToggleBtn.title = 'Sidebar\'ı Kapat';
                sidebarToggleBtn.classList.remove('blinking');
            }
            
            // Save state to localStorage
            localStorage.setItem('sidebarCollapsed', sidebar.classList.contains('collapsed'));
        }
        
        // Button click event (direct toggle)
        sidebarToggleBtn.addEventListener('click', function(e) {
            e.preventDefault();
            toggleSidebar();
        });
        
        // Load saved state
        const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
        if (isCollapsed) {
            sidebar.classList.add('collapsed');
            mainContent.classList.add('expanded');
            sidebarToggleIcon.className = 'bi bi-layout-sidebar sidebar-icon-collapsed';
            sidebarToggleBtn.title = 'Sidebar\'ı Aç';
            sidebarToggleBtn.classList.add('blinking');
        }
    }
    
    // User menu toggle
    const userMenuBtn = document.getElementById('userMenuBtn');
    const userMenu = document.getElementById('userMenu');
    
    if (userMenuBtn && userMenu) {
        userMenuBtn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Close other dropdowns first
            const otherDropdowns = document.querySelectorAll('.dropdown-menu.show');
            otherDropdowns.forEach(dropdown => {
                if (dropdown !== userMenu) {
                    dropdown.classList.remove('show');
                }
            });
            
            // Toggle current dropdown
            userMenu.classList.toggle('show');
        });
    }
    
    // Notification menu toggle
    const notificationBtn = document.getElementById('notificationBtn');
    const notificationMenu = document.getElementById('notificationMenu');
    
    if (notificationBtn && notificationMenu) {
        notificationBtn.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            // Close other dropdowns first
            const otherDropdowns = document.querySelectorAll('.dropdown-menu.show');
            otherDropdowns.forEach(dropdown => {
                if (dropdown !== notificationMenu) {
                    dropdown.classList.remove('show');
                }
            });
            
            // Toggle current dropdown
            notificationMenu.classList.toggle('show');
        });
    }
    
    // Sidebar toggle dropdown - REMOVED (artık basit buton)
    
    // Close dropdowns when clicking outside
    document.addEventListener('click', function(e) {
        const allDropdowns = document.querySelectorAll('.dropdown-menu.show');
        allDropdowns.forEach(dropdown => {
            const button = dropdown.previousElementSibling;
            if (button && !button.contains(e.target) && !dropdown.contains(e.target)) {
                dropdown.classList.remove('show');
            }
        });
    });
    
    // Close on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            const allDropdowns = document.querySelectorAll('.dropdown-menu.show');
            allDropdowns.forEach(dropdown => {
                dropdown.classList.remove('show');
            });
        }
    });
    
    // Search functionality
    const searchBtn = document.getElementById('searchBtn');
    const searchInput = document.getElementById('searchInput');
    
    if (searchBtn && searchInput) {
        // Search button click
        searchBtn.addEventListener('click', function(e) {
            e.preventDefault();
            const searchTerm = searchInput.value.trim();
            if (searchTerm) {
                performSearch(searchTerm);
            } else {
                searchInput.focus();
            }
        });
        
        // Enter key in search input
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const searchTerm = searchInput.value.trim();
                if (searchTerm) {
                    performSearch(searchTerm);
                }
            }
        });
        
        // Search input focus effects
        searchInput.addEventListener('focus', function() {
            this.parentElement.classList.add('focused');
        });
        
        searchInput.addEventListener('blur', function() {
            this.parentElement.classList.remove('focused');
        });
    }
    
    // Search function
    function performSearch(searchTerm) {
        console.log('Arama yapılıyor:', searchTerm);
        
        // Burada gerçek arama işlemi yapılabilir
        // Örnek: AJAX isteği, sayfa yönlendirmesi, vs.
        
        // Şimdilik basit bir uyarı gösterelim
        if (typeof showAlert === 'function') {
            showAlert(`"${searchTerm}" için arama yapılıyor...`, 'info');
        }
        
        // Gerçek uygulamada burada arama sonuçlarını gösterebilirsiniz
        // Örnek: window.location.href = `/Search?q=${encodeURIComponent(searchTerm)}`;
    }
    
    // Submenu toggle
    const submenuToggles = document.querySelectorAll('[data-submenu-toggle]');
    submenuToggles.forEach(toggle => {
        toggle.addEventListener('click', function(e) {
            e.preventDefault();
            const submenu = document.getElementById(this.getAttribute('data-submenu-toggle'));
            if (submenu) {
                submenu.classList.toggle('show');
                const arrow = this.querySelector('.submenu-arrow');
                if (arrow) {
                    arrow.classList.toggle('rotated');
                }
            }
        });
    });

    // Nav link click handler - prevent accordion closing
    const navLinks = document.querySelectorAll('.sidebar-nav .nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // Check if this is a submenu toggle (has data-submenu-toggle but no href or href is #)
            if (this.getAttribute('data-submenu-toggle') && (!this.getAttribute('href') || this.getAttribute('href') === '#')) {
                e.preventDefault();
                return;
            }
            
            // For actual navigation links (including main menus with controller/action), set active state
            if (this.getAttribute('href') && this.getAttribute('href') !== '#') {
                // Remove active from all links
                navLinks.forEach(l => l.classList.remove('active'));
                // Add active to clicked link
                this.classList.add('active');
                
                // Open ALL parent accordions for this link
                openAllParentAccordions(this);
            }
        });
    });

    // Clickable icon handler - navigate to controller/action
    const clickableIcons = document.querySelectorAll('.nav-icon-clickable');
    clickableIcons.forEach(icon => {
        icon.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            const controller = this.getAttribute('data-controller');
            const action = this.getAttribute('data-action');
            
            if (controller && action) {
                // Navigate to the controller/action
                window.location.href = `/${controller}/${action}`;
            }
        });
    });

    // Active menu detection and accordion opening (only on page load)
    function setActiveMenu() {
        const currentPath = window.location.pathname;
        const currentController = getControllerFromPath(currentPath);
        const currentAction = getActionFromPath(currentPath);
        
        // Find all nav links
        const allNavLinks = document.querySelectorAll('.sidebar-nav .nav-link');
        
        allNavLinks.forEach(link => {
            // Remove active class from all links
            link.classList.remove('active');
            
            // Check if this link matches current page
            const href = link.getAttribute('href');
            const controller = link.getAttribute('asp-controller');
            const action = link.getAttribute('asp-action');
            
            let isActive = false;
            
            // Check direct href match
            if (href && href !== '#' && currentPath.includes(href)) {
                isActive = true;
            }
            
            // Check controller/action match
            if (controller && action) {
                if (currentController && currentAction) {
                    if (controller.toLowerCase() === currentController.toLowerCase() && 
                        action.toLowerCase() === currentAction.toLowerCase()) {
                        isActive = true;
                    }
                }
            }
            
            // Check controller only match
            if (controller && !action) {
                if (currentController && controller.toLowerCase() === currentController.toLowerCase()) {
                    isActive = true;
                }
            }
            
            if (isActive) {
                link.classList.add('active');
                
                // Open ALL parent accordions to ensure visibility
                openAllParentAccordions(link);
            }
        });
    }
    
    // Helper function to get controller from path
    function getControllerFromPath(path) {
        const segments = path.split('/').filter(segment => segment);
        return segments.length > 0 ? segments[0] : null;
    }
    
    // Helper function to get action from path
    function getActionFromPath(path) {
        const segments = path.split('/').filter(segment => segment);
        return segments.length > 1 ? segments[1] : 'Index';
    }
    
    // Helper function to open parent accordions (only for the specific active link)
    function openParentAccordions(activeLink) {
        // Check if this is a submenu item
        if (activeLink.classList.contains('submenu-item')) {
            // Find parent submenu (level 1)
            let parentSubmenu = activeLink.closest('.submenu');
            if (parentSubmenu) {
                parentSubmenu.classList.add('show');
                
                // Find parent toggle button
                const parentToggle = document.querySelector(`[data-submenu-toggle="${parentSubmenu.id}"]`);
                if (parentToggle) {
                    const arrow = parentToggle.querySelector('.submenu-arrow');
                    if (arrow) {
                        arrow.classList.add('rotated');
                    }
                }
                
                // Check if this is a level-2 submenu item
                if (activeLink.classList.contains('submenu-level-2-item')) {
                    // Find parent level-2 submenu
                    let parentLevel2Submenu = activeLink.closest('.submenu-level-2');
                    if (parentLevel2Submenu) {
                        parentLevel2Submenu.classList.add('show');
                        
                        // Find parent level-2 toggle button
                        const parentLevel2Toggle = document.querySelector(`[data-submenu-toggle="${parentLevel2Submenu.id}"]`);
                        if (parentLevel2Toggle) {
                            const arrow = parentLevel2Toggle.querySelector('.submenu-arrow');
                            if (arrow) {
                                arrow.classList.add('rotated');
                            }
                        }
                    }
                }
            }
        }
        
        // For main menu items (not submenu items), don't close other accordions
        // Just ensure the clicked item's parent accordions are open
        if (!activeLink.classList.contains('submenu-item')) {
            // This is a main menu item, no need to open parent accordions
            return;
        }
    }
    
    // Enhanced function to open ALL parent accordions for a given link
    function openAllParentAccordions(activeLink) {
        // Start from the active link and work backwards
        let currentElement = activeLink;
        
        // Find all parent submenus and open them
        while (currentElement) {
            // Check if current element is inside a submenu
            const parentSubmenu = currentElement.closest('.submenu');
            if (parentSubmenu) {
                parentSubmenu.classList.add('show');
                
                // Find and rotate the toggle button
                const parentToggle = document.querySelector(`[data-submenu-toggle="${parentSubmenu.id}"]`);
                if (parentToggle) {
                    const arrow = parentToggle.querySelector('.submenu-arrow');
                    if (arrow) {
                        arrow.classList.add('rotated');
                    }
                }
                
                // Move up to the next level
                currentElement = parentToggle;
            } else {
                break;
            }
        }
    }
    
    // Set active menu on page load
    setActiveMenu();
    
    // Set active menu on browser back/forward
    window.addEventListener('popstate', function() {
        setActiveMenu();
    });
    
    // Close dropdowns on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            // Close user menu
            if (userMenu && userMenu.classList.contains('show')) {
                userMenu.classList.remove('show');
            }
            
            // Close sidebar on mobile
            if (window.innerWidth <= 768 && sidebar && sidebar.classList.contains('show')) {
                sidebar.classList.remove('show');
                if (sidebarOverlay) {
                    sidebarOverlay.classList.remove('show');
                }
            }
        }
    });
    
    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('show');
            if (sidebarOverlay) {
                sidebarOverlay.classList.remove('show');
            }
        }
    });
    
    // Initialize tooltips if Bootstrap is available
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }
    
    // Initialize popovers if Bootstrap is available
    if (typeof bootstrap !== 'undefined' && bootstrap.Popover) {
        const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }
});

// Utility functions
function showAlert(message, type = 'info') {
    const alertContainer = document.getElementById('alertContainer') || createAlertContainer();
    const alertId = 'alert-' + Date.now();
    
    const alertHtml = `
        <div id="${alertId}" class="alert alert-${type} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    alertContainer.insertAdjacentHTML('beforeend', alertHtml);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        const alert = document.getElementById(alertId);
        if (alert) {
            alert.remove();
        }
    }, 5000);
}

function createAlertContainer() {
    const container = document.createElement('div');
    container.id = 'alertContainer';
    container.className = 'position-fixed top-0 end-0 p-3';
    container.style.zIndex = '9999';
    document.body.appendChild(container);
    return container;
}

// Form validation helpers
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return false;
    
    const inputs = form.querySelectorAll('input[required], select[required], textarea[required]');
    let isValid = true;
    
    inputs.forEach(input => {
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        } else {
            input.classList.remove('is-invalid');
        }
    });
    
    return isValid;
}

// Loading state helpers
function setLoadingState(element, loading = true) {
    if (loading) {
        element.disabled = true;
        element.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Yükleniyor...';
    } else {
        element.disabled = false;
        element.innerHTML = element.getAttribute('data-original-text') || 'Gönder';
    }
}
