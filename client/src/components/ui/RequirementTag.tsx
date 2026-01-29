interface Props {
  requirement: string;
}

export default function RequirementTag({ requirement }: Props) {
  return (
    <span
      key={requirement}
      className="bg-cream-warm hover:text-accent hover:bg-accent-light/15 rounded-full px-3 py-1 text-sm font-medium text-stone-500 transition-colors duration-200"
    >
      {requirement}
    </span>
  );
}
