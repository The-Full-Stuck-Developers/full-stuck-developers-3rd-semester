import React from "react";

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
}

const Pagination: React.FC<PaginationProps> = ({currentPage, totalPages, onPageChange}) => {
    const pages: number[] = [];

    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);

    for (let i = startPage; i <= endPage; i++) {
        pages.push(i);
    }

    const handlePrev = () => {
        if (currentPage > 1) onPageChange(currentPage - 1);
    };

    const handleNext = () => {
        if (currentPage < totalPages) onPageChange(currentPage + 1);
    };

    return (
        <div className="join">
            <button
                className="join-item btn btn-outline"
                onClick={handlePrev}
                disabled={currentPage === 1}
            >
                Prev
            </button>

            {startPage > 1 && (
                <>
                    <button className="join-item btn btn-outline" onClick={() => onPageChange(1)}>
                        1
                    </button>
                    {startPage > 2 && <span className="join-item btn btn-ghost cursor-default">...</span>}
                </>
            )}

            {pages.map((page) => (
                <button
                    key={page}
                    className={`join-item btn ${page === currentPage ? "btn-active" : "btn-outline"}`}
                    onClick={() => onPageChange(page)}
                >
                    {page}
                </button>
            ))}

            {endPage < totalPages && (
                <>
                    {endPage < totalPages - 1 && <span className="join-item btn btn-ghost cursor-default">...</span>}
                    <button className="join-item btn btn-outline" onClick={() => onPageChange(totalPages)}>
                        {totalPages}
                    </button>
                </>
            )}

            <button
                className="join-item btn btn-outline"
                onClick={handleNext}
                disabled={currentPage === totalPages}
            >
                Next
            </button>
        </div>
    );
};

export default Pagination;
