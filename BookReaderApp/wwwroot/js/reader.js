window.readerFunctions = {
    objRef: null,

    // Initialize the reader functions
    init: function(dotNetObjectReference) {
        this.objRef = dotNetObjectReference;
        this.setupAutoSave();
    },

    // Text-to-Speech functionality
    startTextToSpeech: function(text, rate = 1, pitch = 1) {
        if ('speechSynthesis' in window) {
            // Stop any existing speech
            window.speechSynthesis.cancel();
            
            const utterance = new SpeechSynthesisUtterance(text);
            utterance.rate = rate;
            utterance.pitch = pitch;
            utterance.lang = 'pl-PL';
            
            utterance.onend = () => {
                if (this.objRef) {
                    this.objRef.invokeMethodAsync('OnSpeechEnded');
                }
            };
            
            utterance.onerror = (event) => {
                console.error('Speech synthesis error:', event.error);
                if (this.objRef) {
                    this.objRef.invokeMethodAsync('OnSpeechEnded');
                }
            };
            
            window.speechSynthesis.speak(utterance);
            return true;
        }
        console.warn('Speech synthesis not supported');
        return false;
    },
    
    stopTextToSpeech: function() {
        if ('speechSynthesis' in window) {
            window.speechSynthesis.cancel();
        }
    },
    
    // Reading position tracking
    saveScrollPosition: function() {
        const container = document.querySelector('.reader-content');
        if (container) {
            return container.scrollTop;
        }
        return 0;
    },
    
    restoreScrollPosition: function(position) {
        const container = document.querySelector('.reader-content');
        if (container) {
            container.scrollTop = position;
        }
    },
    
    // Text selection
    getSelectedText: function() {
        const selection = window.getSelection();
        const text = selection.toString().trim();
        // Limit selected text to reasonable length
        return text.length > 500 ? text.substring(0, 500) + '...' : text;
    },
    
    // Calculate reading progress
    calculateReadingProgress: function() {
        const container = document.querySelector('.reader-content');
        if (container) {
            const scrollTop = container.scrollTop;
            const scrollHeight = container.scrollHeight;
            const clientHeight = container.clientHeight;
            
            if (scrollHeight <= clientHeight) return 100;
            
            const progress = (scrollTop / (scrollHeight - clientHeight)) * 100;
            return Math.min(100, Math.max(0, progress));
        }
        return 0;
    },

    // Auto-save setup
    setupAutoSave: function() {
        // Auto-save reading position every 10 seconds
        if (this.autoSaveInterval) {
            clearInterval(this.autoSaveInterval);
        }
        
        this.autoSaveInterval = setInterval(() => {
            if (this.objRef) {
                const position = this.saveScrollPosition();
                const progress = this.calculateReadingProgress();
                
                // Only call if there's meaningful change
                if (position > 0 || progress > 0) {
                    console.log('Auto-save position:', position, 'progress:', progress);
                }
            }
        }, 10000);
    },

    // Utility functions
    scrollToPosition: function(position) {
        const container = document.querySelector('.reader-content');
        if (container) {
            container.scrollTo({
                top: position,
                behavior: 'smooth'
            });
        }
    },

    // Clean up
    dispose: function() {
        this.stopTextToSpeech();
        if (this.autoSaveInterval) {
            clearInterval(this.autoSaveInterval);
        }
        this.objRef = null;
    }
};

// Keyboard shortcuts
document.addEventListener('keydown', function(event) {
    // Only apply shortcuts when reading
    if (!document.querySelector('.reader-content')) return;
    
    // Prevent shortcuts when typing in inputs
    if (event.target.tagName === 'INPUT' || event.target.tagName === 'TEXTAREA') return;
    
    switch(event.key) {
        case 'b':
        case 'B':
            if (event.ctrlKey || event.metaKey) {
                event.preventDefault();
                console.log('Bookmark shortcut pressed');
            }
            break;
        case 'n':
        case 'N':
            if (event.ctrlKey || event.metaKey) {
                event.preventDefault();
                console.log('Note shortcut pressed');
            }
            break;
        case ' ':
            if (event.ctrlKey || event.metaKey) {
                event.preventDefault();
                console.log('Text-to-speech shortcut pressed');
            }
            break;
        case '+':
        case '=':
            if (event.ctrlKey || event.metaKey) {
                event.preventDefault();
                console.log('Increase font size shortcut pressed');
            }
            break;
        case '-':
            if (event.ctrlKey || event.metaKey) {
                event.preventDefault();
                console.log('Decrease font size shortcut pressed');
            }
            break;
    }
});

// Handle page visibility change (for auto-save when user leaves tab)
document.addEventListener('visibilitychange', function() {
    if (document.hidden && window.readerFunctions.objRef) {
        const position = window.readerFunctions.saveScrollPosition();
        const progress = window.readerFunctions.calculateReadingProgress();
        console.log('Page hidden - saving position:', position, 'progress:', progress);
    }
});

// Clean up on page unload
window.addEventListener('beforeunload', function() {
    if (window.readerFunctions) {
        window.readerFunctions.dispose();
    }
});