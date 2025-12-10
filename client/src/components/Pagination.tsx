import React from "react";
import {useTranslation} from "react-i18next";

interface PaginationProps {
    currentPage: number;
    totalPages: number;
    onPageChange: (page: number) => void;
}

const Pagination: React.FC<PaginationProps> = ({currentPage, totalPages, onPageChange}) => {
    const {t} = useTranslation();

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
        <div className=" flex h-full">
            <button
                className={`${currentPage === 1 ? "hidden" : "block"} w-20 bg-[#0f2b5b]! text-white cursor-not-allowed rounded-s-lg`}
                onClick={handlePrev}
                disabled={currentPage === 1}
            >
                {t("prev")}
            </button>

            {startPage > 1 && (
                <>
                    <button className="size-10 hover:bg-slate-200 cursor-pointer border border-[#0f2b5b]"
                            onClick={() => onPageChange(1)}>
                        1
                    </button>
                    {startPage > 2 && <span
                        className={"size-10 text-center align-middle cursor-default border border-[#0f2b5b] flex items-center justify-center pb-2.5"}>...</span>}
                </>
            )}

            {pages.map((page) => (
                <button
                    key={page}
                    className={`size-10  ${page === currentPage ? "bg-[#0f2b5b] text-white cursor-not-allowed" : "hover:bg-slate-200 cursor-pointer border border-[#0f2b5b]"}`}
                    onClick={() => onPageChange(page)}
                >
                    {page}
                </button>
            ))}

            {endPage < totalPages && (
                <>
                    {endPage < totalPages - 1 && <span
                        className="size-10 text-center align-middle cursor-default border border-[#0f2b5b] flex items-center justify-center pb-2.5">...</span>}
                    <button className="size-10 hover:bg-slate-200 cursor-pointer border border-[#0f2b5b]"
                            onClick={() => onPageChange(totalPages)}>
                        {totalPages}
                    </button>
                </>
            )}

            <button
                className={`${currentPage === totalPages ? "hidden" : "block"} w-20 bg-[#0f2b5b] text-white cursor-pointer rounded-e-lg `}
                onClick={handleNext}
                disabled={currentPage === totalPages}
            >
                {t("next")}
            </button>
        </div>
    );
};

export default Pagination;
